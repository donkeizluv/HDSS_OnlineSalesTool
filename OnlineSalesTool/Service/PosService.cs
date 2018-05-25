using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Helper;
using OnlineSalesTool.POCO;
using OnlineSalesTool.Query;
using OnlineSalesTool.Service;
using OnlineSalesTool.ViewModels;

namespace OnlineSalesTool.Service
{
    public class PosService : ServiceBase, IPosService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ListQuery<Pos, PosPOCO> _query;

        public PosService(OnlineSalesContext context,
            IUserResolver userResolver,
            ListQuery<Pos, PosPOCO> q)
            : base(userResolver.GetPrincipal(), context)
        {
            _query = q;
        }

        public async Task<PosListingVM> Get(ListingParams param)
        {
            var vm = new PosListingVM(param ?? throw new ArgumentNullException());
            var q = _query.CreateBaseQuery(Role.ToString());
            (var items, int total) = await _query.ApplyParameters(q, param);
            vm.SetItems(items, param.ItemPerPage, total);
            return vm;
        }
        
        public Task Create(PosPOCO pos)
        {
            throw new NotImplementedException();
        }

        public Task Update(PosPOCO pos)
        {
            throw new NotImplementedException();
        }
    }
}
