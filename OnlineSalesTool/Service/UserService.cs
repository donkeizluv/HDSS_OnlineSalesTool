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
using OnlineSalesTool.POCO;
using OnlineSalesTool.Query;
using OnlineSalesTool.ViewModels;

namespace OnlineSalesTool.Service
{
    public class UserService : ServiceBase, IUserService
    {
        private const int SUGGEST_TAKE = 10;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ListQuery<AppUser, AppUserPOCO> _query;
        private readonly GeneralOptions _options;
        private readonly IRoleCache _roleCache;

        public UserService(OnlineSalesContext context,
            IUserResolver userResolver,
            ListQuery<AppUser, AppUserPOCO> q,
            IOptions<GeneralOptions> options,
            IRoleCache roleCache)
            : base(userResolver.GetPrincipal(), context)
        {
            _query = q;
            _options = options.Value;
            _roleCache = roleCache;
        }

        public async Task<int> Create(AppUserPOCO user)
        {
            if (user == null) throw new ArgumentNullException();
            await DbContext.AppUser.AddAsync(await ToNewUser(user));
            return await DbContext.SaveChangesAsync();
        }
        private void BasicCheck(AppUserPOCO user)
        {
            if (user == null) throw new ArgumentNullException();
            //Check for null
            if (string.IsNullOrEmpty(user.Name)) throw new BussinessException("Missing value");
            if (string.IsNullOrEmpty(user.Username)) throw new BussinessException("Missing value");
            if (string.IsNullOrEmpty(user.HR)) throw new BussinessException("Missing value");
            if (string.IsNullOrEmpty(user.Role)) throw new BussinessException("Missing value");
        }
        private async Task<AppUser> ToNewUser(AppUserPOCO user)
        {
            BasicCheck(user);
            //Check role & get role id
            _roleCache.GetRoleId(user.Role, out int roleId, out var appRole);
            //CA must have manager
            if(appRole == RoleEnum.CA)
                if(user.Manager == null) throw new BussinessException("CA Role must have manager");
            //Check if Manager is valid
            await CheckManager(user);
            return new AppUser()
            {
                Name = user.Name,
                Username = user.Username,
                Active = true,
                Email = $"{user.Username}{_options.EmailSuffix}",
                Hr = user.HR,
                Phone = user.Phone,
                Phone2 = user.Phone2,
                RoleId = roleId,
                ManagerId = user.Manager?.UserId
            };
        }
        private async Task CheckManager(AppUserPOCO user)
        {
            if (user.Manager != null)
            {
                if (!await DbContext.AppUser
                .AnyAsync(u => u.UserId == user.Manager.UserId && //Manager exists?
                    u.Role.Name != RoleEnum.CA.ToString())) //CA cant be manager
                    throw new BussinessException($"Manager id:{user.Manager.UserId} is not exist or not in valid roles");
            }
        }
        private async Task<AppUser> ApplyUpdate(AppUserPOCO user)
        {
            BasicCheck(user);
            await CheckManager(user);
            var appUser = await DbContext.AppUser.SingleOrDefaultAsync(u => u.UserId == user.UserId);
            if (appUser == null)
                throw new BussinessException($"Invalid user id: {user.UserId}");
            appUser.Name = user.Name;
            appUser.Username = user.Username;
            appUser.Hr = user.HR;
            appUser.Active = user.Active;
            appUser.ManagerId = user.Manager.UserId;
            //Currently, not allowing update user's role
            //appUser.Role = user.Role
            appUser.Phone = user.Phone;
            appUser.Phone2 = user.Phone2;
            return appUser;
        }

        public async Task<UserListingVM> Get(ListingParams param)
        {
            var vm = new UserListingVM(param ?? throw new ArgumentNullException());
            var q = _query.CreateBaseQuery();
            (var items, int total) = await _query.ApplyParameters(q, param);
            vm.SetItems(items, param.ItemPerPage, total);
            return vm;
        }

        public async Task<IEnumerable<SelectOptionPOCO>> SearchSuggest(RoleEnum role, string q)
        {
            return await DbContext.AppUser
                .Where(u => (u.Username.Contains(q) || u.Hr.Contains(q)) && u.Role.Name == role.ToString())
                .Take(SUGGEST_TAKE)
                .Select(u => new SelectOptionPOCO() {
                    label = $"{u.Username} - {u.Hr}",
                    value = u.UserId
                }).ToListAsync();
        }
        //Currently, not allowing update user's role
        public async Task Update(AppUserPOCO user)
        {
            if (user == null) throw new ArgumentNullException();
            var appUser = await ApplyUpdate(user);
            await DbContext.SaveChangesAsync();
        }
    }
}
