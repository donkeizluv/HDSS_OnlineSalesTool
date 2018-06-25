using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.ApiParameter;
using OnlineSalesCore.Cache;
using OnlineSalesCore.Const;
using OnlineSalesCore.DTO;
using OnlineSalesCore.EFModel;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Extention;
using OnlineSalesCore.Query;
using OnlineSalesCore.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesCore.Service
{
    public class CaseService : ServiceBase, ICaseService
    {
        private readonly ILogger _logger;
        private readonly ListQuery<OnlineOrder, CaseDTO> _query;
        private readonly IRoleCache _roleCache;

        public CaseService(OnlineSalesContext context,
            IHttpContextAccessor httpContext,
            IRoleCache roleCache,
            ILogger<CaseService> logger,
            ListQuery<OnlineOrder, CaseDTO> q)
            : base(httpContext, context)
        {
            _logger = logger;
            _query = q;
            _roleCache = roleCache;
        }

        public async Task<CaseListingVM> Get(ListingParams param)
        {
            var vm = new CaseListingVM(param ?? throw new ArgumentNullException());
            var q = _query.CreateBaseQuery(Role.ToString());
            (var items, int total) = await _query.ApplyParameters(q, param);
            vm.SetItems(items, param.ItemPerPage, total);
            //Availabe shifts
            return vm;
        }
    }
}
