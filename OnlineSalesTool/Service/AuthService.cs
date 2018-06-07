using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineSalesTool.Auth;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Options;
using OnlineSalesTool.Const;
using System;
using System.Threading.Tasks;
using static OnlineSalesTool.Controllers.AccountController;

namespace OnlineSalesTool.Const
{
    public class AuthService : ServiceBase, IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly WindowsAuthOptions _authOption;

        public AuthService(OnlineSalesContext context,
            IHttpContextAccessor httpContext,
            ILogger<AuthService> logger,
            IOptions<WindowsAuthOptions> option)
            : base(httpContext, context)
        {
            _logger = logger;
            _authOption = option.Value;
        }

        public async Task<(LoginResult, AppUser)> Authenticate(string userName, string pwd)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
                return (LoginResult.Error, null);
            //Validate against AD
            if (!Validate(userName, pwd)) return (LoginResult.Error, null);
            var user = await GetUser(userName);
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

        public async Task<AppUser> GetUser(string userName)
        {
            //Include everything needed to be added in claims
            var user = await DbContext.AppUser
                .Include(u => u.UserAbility)
                    .ThenInclude(ua => ua.Ability)
                .Include(u => u.Role)
               .FirstOrDefaultAsync(u => u.Username == userName);
            user.LastLogin = DateTime.Now;
            //Save last login
            await DbContext.SaveChangesAsync();
            return user;
        }
    }
}
