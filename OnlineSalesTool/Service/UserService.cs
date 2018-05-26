using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.AppEnum;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.POCO;
using OnlineSalesTool.Query;
using OnlineSalesTool.Service;
using OnlineSalesTool.ViewModels;

namespace OnlineSalesTool.Service
{
    public class UserService : ServiceBase, IUserService
    {
        private const int SUGGEST_TAKE = 10;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ListQuery<AppUser, AppUserPOCO> _query;

        public UserService(OnlineSalesContext context,
            IUserResolver userResolver,
            ListQuery<AppUser, AppUserPOCO> q)
            : base(userResolver.GetPrincipal(), context)
        {
            _query = q;
        }

        public Task Create(AppUserPOCO pos)
        {
            throw new NotImplementedException();
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
                    value = UserId
                }).ToListAsync();
        }

        public Task Update(AppUserPOCO pos)
        {
            throw new NotImplementedException();
        }
    }
}
