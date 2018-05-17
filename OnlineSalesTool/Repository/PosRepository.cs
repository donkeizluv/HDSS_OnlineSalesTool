using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.POCO;
using OnlineSalesTool.Service;
using OnlineSalesTool.ViewModels;

namespace OnlineSalesTool.Repository
{
    public class PosRepository : BaseRepo, IPosRepository
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PosRepository(OnlineSalesContext context, IUserResolver userResolver)
            : base(userResolver.GetPrincipal(), context) { }

        public async Task<PosListingVM> Get(ListingParams param)
        {
            var checkedFilter = string.IsNullOrEmpty(param.Filter) ? nameof(Pos.PosName) : param.Filter;
            var vm = new PosListingVM()
            {
                FilterBy = checkedFilter,
                FilterString = param.Contain,
                OnPage = param.Page,
                OrderAsc = param.Asc,
                OrderBy = param.OrderBy,
            };
            (var items, var total) = await CreateItems(param, vm.ItemPerPage);
            vm.TotalRows = total;
            vm.Items = items;
            return vm;
        }
        private IQueryable<Pos> CreateBaseQuery()
        {
            switch (Role)
            {
                case AppEnum.RoleEnum.CA:
                    return CA_Query();
                case AppEnum.RoleEnum.BDS:
                    return BDS_Query();
                case AppEnum.RoleEnum.ASM:
                    throw new NotImplementedException();
                case AppEnum.RoleEnum.ADMIN:
                    return ADMIN_Query();
                default:
                    throw new InvalidOperationException();
            }
        }

        #region queries
        /// <summary>
        /// Returns POS under management of this CA's manager
        /// </summary>
        /// <returns></returns>
        private IQueryable<Pos> CA_Query()
        {
            var man = DbContext.AppUser
                .Where(u => u.UserId == UserId)
                .Include(u => u.Manager)
                    .ThenInclude(u => u.Pos)
                        .ThenInclude(p => p.PosShift)
                            .ThenInclude(p => p.Shift)
                .FirstOrDefault().Manager;
            if(man == null)
            {
                //Invalid
                _logger.Debug($"CA UserId: {UserId} doesnt have manager!");
                //Return empty POS since we dont know which POS this user belongs to

                //This doesnt imlp IAsyncEnumerable so when await later will cause exception
                return new EnumerableQuery<Pos>(new[] { new Pos() });
                //return null;
            }
            return DbContext.Pos.Where(p => p.UserId == man.UserId);
        }
        /// <summary>
        /// Returns all POS under this user management
        /// </summary>
        /// <returns></returns>
        private IQueryable<Pos> BDS_Query()
        {
            return DbContext.Pos
                .Where(p => p.UserId == UserId)
                .Include(p => p.User)
                .Include(p => p.PosShift)
                    .ThenInclude(ps => ps.Shift);
        }
        /// <summary>
        /// Returns all POS
        /// </summary>
        /// <returns></returns>
        private IQueryable<Pos> ADMIN_Query()
        {
            return DbContext.Pos
                .Include(p => p.User)
                .Include(p => p.PosShift)
                    .ThenInclude(ps => ps.Shift); ;
        }
        #endregion

        private async Task<(IEnumerable<PosPOCO>, int)> CreateItems(ListingParams apiParam, int take)
        {
            //TODO: Fluent interface?
            int excludedRows = (apiParam.Page - 1) * take;
            var q = CreateBaseQuery();
            int total = 0;
            //Apply filter if possible
            if (!string.IsNullOrEmpty(apiParam.Filter) && !string.IsNullOrEmpty(apiParam.Contain))
            {
                q = ApplyFilter(q, apiParam.Filter, apiParam.Contain);
                total = q.Count();
            }
            else
                total = q.Count();

            //Apply order
            q = ApplyOrder(q, apiParam.OrderBy, apiParam.Asc).Skip(excludedRows).Take(take);
            //Project to PosPoco
            var projection = q.Select(p => new PosPOCO(p) { BDS = new AppUserPOCO(p.User) });
            //Check if query is awaitable
            //https://expertcodeblog.wordpress.com/2018/02/19/net-core-2-0-resolve-error-the-source-iqueryable-doesnt-implement-iasyncenumerable/
            if (projection is IAsyncEnumerable<PosPOCO>)
                return (await projection.ToListAsync(), total);
            return (projection.ToList(), total);
        }
        private IOrderedQueryable<Pos> ApplyOrder(IQueryable<Pos> query, string order, bool asc)
        {
            switch (order)
            {
                case nameof(Pos.PosName):
                    if (!asc)
                        return query.OrderByDescending(r => r.PosName);
                    return query.OrderBy(r => r.PosName);
                case nameof(Pos.PosCode):
                    if (!asc)
                        return query.OrderByDescending(r => r.PosCode);
                    return query.OrderBy(r => r.PosCode);
                case "BDS": 
                    if (!asc)
                        return query.OrderByDescending(r => r.User.Username);//Does this work?
                    return query.OrderBy(r => r.User.Username);
                default:
                    _logger.Debug($"Unknown <{nameof(order)}> value: {order}");
                    return query.OrderByDescending(r => r.PosName);
            }
        }
        private IQueryable<Pos> ApplyFilter(IQueryable<Pos> query, string filterBy, string filterText)
        {
            switch (filterBy)
            {
                case nameof(Pos.PosName):
                    return query.Where(c => c.PosName.Contains(filterText));
                case nameof(Pos.PosCode):
                    return query.Where(c => c.PosCode.Contains(filterText));
                case nameof(Pos.Phone):
                    return query.Where(c => c.Phone.Contains(filterText));
                case "BDS": 
                    return query.Where(c => c.User.Username.Contains(filterText));//Does this work?
                default:
                    _logger.Debug($"Unknown <{nameof(filterBy)}> value: {filterBy}");
                    return query;
            }
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
