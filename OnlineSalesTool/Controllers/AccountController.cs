﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlineSalesCore.Const;
using OnlineSalesCore.Models;
using OnlineSalesCore.Options;
using OnlineSalesCore.Services;
using OnlineSalesTool.AuthToken;
using OnlineSalesTool.Filter;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(AccountController))]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        private readonly IAuthService _service;

        public AccountController(IConfiguration config,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            ILogger<AccountController> logger,
            IAuthService service)
        {
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
            _service = service;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]string username = "", [FromForm]string pwd = "")
        {
            (var level, var user) = await _service.Authenticate(username, pwd);
            if (level == LoginResult.Error ||
            level == LoginResult.NoPermission ||
            level == LoginResult.NotActive)
                return Forbid();
            //OK proceed
            //add claims to identity
            var userIdentity = new ClaimsIdentity();
            //Add whatever claim user has here
            userIdentity.AddClaims(_service.CreateClaims(user));
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

    }
}
