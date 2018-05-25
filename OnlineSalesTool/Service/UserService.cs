using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.POCO;
using OnlineSalesTool.Query;
using OnlineSalesTool.Service;
using OnlineSalesTool.ViewModels;

namespace OnlineSalesTool.Service
{
    public class UserService : ServiceBase, IUserService
    {
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

        public Task Update(AppUserPOCO pos)
        {
            throw new NotImplementedException();
        }
    }
}
