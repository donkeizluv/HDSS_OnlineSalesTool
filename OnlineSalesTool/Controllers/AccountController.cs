using OnlineSalesTool.Auth;
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
using OnlineSalesTool.Repository;
using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using OnlineSalesTool.Service;
using System.Linq;
using OnlineSalesTool.Options;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute]
    public class AccountController : Controller
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly AuthenticationOptions _authOption;
        private readonly IAccountRepository _repo;

        public AccountController(IConfiguration config,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IOptions<AuthenticationOptions> authOptions,
            IAccountRepository repo)
        {
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _authOption = authOptions.Value;
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
            (LoginResult level, AppUser user) = await GetLoginLevel(username, pwd);
            if (level == LoginResult.Error) return Unauthorized();
            if (level == LoginResult.NoPermission) return Unauthorized();
            if (level == LoginResult.NotActive) return Unauthorized();
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
                    new Claim(CustomClaims.Username , user.Username.ToLower()),
                    new Claim(CustomClaims.UserId, user.UserId.ToString()),
                };
            //add abilities to claims
            claims.AddRange(user.UserAbility.Select(a => new Claim(CustomClaims.Ability, a.Ability.Name)));
            return claims;
        }
        
        private async Task<(LoginResult, AppUser)> GetLoginLevel(string userName, string pwd)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
                return (LoginResult.Error, null);
            if (!Validate(userName, pwd)) return (LoginResult.Error, null);
            //Includes everything needs to be added to Claims
            var user = await _repo.GetUser(userName);
            if (user == null)
                return (LoginResult.NoPermission, null); //no permission
            if (!user.Active)
                return (LoginResult.NotActive, null);
            return (LoginResult.OK, user);
        }

        private bool Validate(string userName, string pwd)
        {
            if (_authOption.NoPwdCheck) return true;
            return WindowsAuth.Validate_Principal2(userName, pwd, _authOption.Domain);
        }
    }
}
