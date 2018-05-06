using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Service;
using System;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    public class AccountRepository : BaseRepo, IAccountRepository
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AccountRepository(OnlineSalesContext context, IUserResolver userResolver)
            : base(userResolver.GetPrincipal(), context){}

        public async Task<AppUser> GetUser(string userName)
        {
            var user = await DbContext.AppUser.Include(u => u.UserAbility)
                .ThenInclude(ua => ua.Ability)
               .FirstOrDefaultAsync(u => u.Username == userName);
            user.LastLogin = DateTime.Now;
            //Save last login
            await DbContext.SaveChangesAsync();
            return user;
        }
    }
}
