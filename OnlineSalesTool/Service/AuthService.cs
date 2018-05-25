using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using OnlineSalesTool.Auth;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Options;
using OnlineSalesTool.Service;
using System;
using System.Threading.Tasks;
using static OnlineSalesTool.Controllers.AccountController;

namespace OnlineSalesTool.Service
{
    public class AuthService : ServiceBase, IAuthService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly WindowsAuthOptions _authOption;

        public AuthService(OnlineSalesContext context,
            IUserResolver userResolver,
            IOptions<WindowsAuthOptions> option)
            : base(userResolver.GetPrincipal(), context)
        {
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
