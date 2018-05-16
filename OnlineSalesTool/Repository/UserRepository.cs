using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Service;
using System;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    public class UserRepository : BaseRepo, IUserRepository
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserRepository(OnlineSalesContext context, IUserResolver userResolver)
            : base(userResolver.GetPrincipal(), context){}

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
