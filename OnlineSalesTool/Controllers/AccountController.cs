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

namespace OnlineSalesTool.Controllers
{
    [CustomExceptionFilterAttribute]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IAccountRepository _repo;

        internal string Issuer
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<string>("Issuer");
            }
        }
        internal bool NoPwdCheck
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<bool>("NoPwdCheck");
            }
        }

        internal string Domain
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<string>("Domain");
            }
        }

        public AccountController(IConfiguration config,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IAccountRepository repo)
        {
            _config = config;
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
        public async Task<IActionResult> DoLogin([FromForm]string username = "", [FromForm]string pwd = "")
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
        
        private IEnumerable<Claim> CreateClaimSet(AppUser user)
        {
            var claims = new List<Claim>
                {
                    new Claim(CustomClaims.Username.ToString() , user.Username.ToLower()),
                    new Claim(CustomClaims.UserId.ToString(), user.UserId.ToString()),
                };
            //add abilities to claims
            claims.AddRange(user.UserAbility.Select(a => new Claim(AppConst.AbilityClaimName, a.Ability.Name)));
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
            if (NoPwdCheck) return true;
            return WindowsAuth.Validate_Principal2(userName, pwd, Domain);
        }
    }
}
