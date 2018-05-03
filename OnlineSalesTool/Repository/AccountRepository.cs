using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    public class AccountRepository : BaseRepo, IAccountRepository
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly OnlineSalesContext _context;

        public AccountRepository(OnlineSalesContext context, IUserResolver userResolver) : base(userResolver.GetPrincipal())
        {
            _context = context;
        }

        public void Dispose()
        {
            if (_context != null) _context.Dispose();
        }

        public async Task<AppUser> GetUser(string userName)
        {
            var user = await _context.AppUser.Include(u => u.UserAbility)
                .ThenInclude(ua => ua.Ability)
               .FirstOrDefaultAsync(u => u.Username == userName);
            user.LastLogin = DateTime.Now;
            //Save last login
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
