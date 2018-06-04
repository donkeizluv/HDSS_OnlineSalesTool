using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.AppEnum;
using OnlineSalesTool.Cache;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Options;
using OnlineSalesTool.DTO;
using OnlineSalesTool.Query;
using OnlineSalesTool.ViewModels;

namespace OnlineSalesTool.Service
{
    public class UserService : ServiceBase, IUserService
    {
        private const int SUGGEST_TAKE = 5;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ListQuery<AppUser, AppUserDTO> _query;
        private readonly GeneralOptions _options;
        private readonly IRoleCache _roleCache;

        public UserService(OnlineSalesContext context,
            IUserResolver userResolver,
            IOptions<GeneralOptions> options,
            IRoleCache roleCache,
            ListQuery<AppUser, AppUserDTO> q)
            : base(userResolver.GetPrincipal(), context)
        {
            _query = q;
            _options = options.Value;
            _roleCache = roleCache;
        }

        public async Task<int> Create(AppUserDTO user)
        {
            if(user == null) throw new ArgumentNullException();
            await DbContext.AppUser.AddAsync(await ToNewUser(user ?? throw new ArgumentNullException()));
            return await DbContext.SaveChangesAsync();
        }
        private void BasicCheck(AppUserDTO user)
        {
            if (user == null) throw new ArgumentNullException();
            //Check for null
            if (string.IsNullOrEmpty(user.Name)) throw new BussinessException("Missing value");
            if (string.IsNullOrEmpty(user.Username)) throw new BussinessException("Missing value");
            if (string.IsNullOrEmpty(user.HR)) throw new BussinessException("Missing value");
            if (string.IsNullOrEmpty(user.Role)) throw new BussinessException("Missing value");
        }
        private async Task<AppUser> ToNewUser(AppUserDTO user)
        {
            BasicCheck(user ?? throw new ArgumentNullException());
            //Check role & get role id
            _roleCache.GetRoleId(user.Role, out int roleId, out var appRole);
            //CA must have manager
            if(appRole == RoleEnum.CA)
            {
                //CA must have manager
                await CheckUser(user.Manager?.UserId, RoleEnum.BDS, true);
            }
            if(appRole == RoleEnum.BDS)
            {
                //Currenly, only CA allow to have manager
                if (user.Manager != null)
                    throw new BussinessException($"Only CA can have manager(BDS)");
            }
            return new AppUser()
            {
                Name = user.Name,
                Username = user.Username.ToLower(),
                Active = true,
                Email = $"{user.Username}{_options.EmailSuffix}",
                Hr = user.HR,
                Phone = user.Phone,
                Phone2 = user.Phone2,
                RoleId = roleId,
                ManagerId = user.Manager?.UserId
            };
        }
        private async Task<AppUser> ApplyUpdate(AppUserDTO user)
        {
            BasicCheck(user ?? throw new ArgumentNullException());
            var appUser = await DbContext.AppUser.SingleOrDefaultAsync(u => u.UserId == user.UserId);
            if (appUser == null)
                throw new BussinessException($"Invalid user id: {user.UserId}");
            _roleCache.GetRoleId(user.Role, out int roleId, out var appRole);
            if(appRole == RoleEnum.CA)
            {
                //CA must have manager
                await CheckUser(user.Manager?.UserId, RoleEnum.BDS, true);
            }
            if(appRole == RoleEnum.BDS)
            {
                //Currenly, only CA allow to have manager
                if(user.Manager != null)
                    throw new BussinessException($"Only CA can have manager(BDS)");
            }
            appUser.Name = user.Name;
            appUser.Username = user.Username.ToLower();
            appUser.Hr = user.HR;
            appUser.Active = user.Active;
            appUser.ManagerId = user.Manager?.UserId;
            //Currently, not allowing update user's role
            //appUser.Role = user.Role
            appUser.Phone = user.Phone;
            appUser.Phone2 = user.Phone2;
            return appUser;
        }

        public async Task<UserListingVM> Get(ListingParams param)
        {
            var vm = new UserListingVM(param ?? throw new ArgumentNullException());
            var q = _query.CreateBaseQuery(Role.ToString());
            (var items, int total) = await _query.ApplyParameters(q, param);
            vm.SetItems(items, param.ItemPerPage, total);
            return vm;
        }

        public async Task<IEnumerable<AppUserDTO>> SearchSuggest(RoleEnum role, string q)
        {
            return await DbContext.AppUser
                .Where(u => (u.Username.Contains(q) || u.Hr.Contains(q)) 
                    && u.Role.Name == role.ToString() 
                    && u.Active)
                .Take(SUGGEST_TAKE)
                .Select(u => new AppUserDTO(u) ).ToListAsync();
        }
        public async Task Update(AppUserDTO user)
        {
            var appUser = await ApplyUpdate(user ?? throw new ArgumentNullException());
            await DbContext.SaveChangesAsync();
        }

        public async Task<int> CheckUsername(string userName)
        {
            var user = await DbContext.AppUser.FirstOrDefaultAsync(p => p.Username == userName);
            if(user == null) return -1;
            return user.UserId;
        }
    }
}
