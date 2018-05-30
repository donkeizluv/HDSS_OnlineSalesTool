using OnlineSalesTool.Filter;
using OnlineSalesTool.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineSalesTool.Logic;
using System.Security.Claims;
using System.Threading.Tasks;
using OnlineSalesTool.Service;
using OnlineSalesTool.EFModel;
using System.Collections.Generic;
using System.Linq;
using OnlineSalesTool.Options;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute]
    public class AccountController : Controller
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        
        private readonly IAuthService _repo;

        public AccountController(IConfiguration config,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IAuthService repo)
        {
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _repo = repo;
        }

        public enum LoginResult
        {
            Error,
            NotActive,
            NoPermission,
            OK
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]string username = "", [FromForm]string pwd = "")
        {
            //Check against db or whatever here
            (LoginResult level, AppUser user) = await _repo.Authenticate(username, pwd);
            if (level == LoginResult.Error) return Forbid();
            if (level == LoginResult.NoPermission) return Forbid();
            if (level == LoginResult.NotActive) return Forbid();
            //OK proceed
            //add claims to identity
            var userIdentity = new ClaimsIdentity();
            //Add whatever claim user has here
            userIdentity.AddClaims(CreateClaimSet(user));
            //Create token
            var jwt = await Token.GenerateJwt(userIdentity,
                _jwtFactory,
                user.Username,
                _jwtOptions,
                new JsonSerializerSettings { Formatting = Formatting.Indented });
            //Return token
            return Ok(jwt);
        }
        
        [HttpGet]
        [Authorize]
        //Use this to check if token still valid
        public IActionResult Ping()
        {
            return Ok();
        }

        private IEnumerable<Claim> CreateClaimSet(AppUser user)
        {
            var claims = new List<Claim>
                {
                    new Claim(CustomClaims.UserId, user.UserId.ToString()),
                    new Claim(CustomClaims.UserRole, user.Role.Name),
                    new Claim(CustomClaims.Username, user.Username.ToLower())
                };
            //add abilities to claims
            claims.AddRange(user.UserAbility.Select(a => new Claim(CustomClaims.Ability, a.Ability.Name)));
            return claims;
        }
     
    }
}
