using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.Helper;
using OnlineSalesCore.Const;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Models;
using OnlineSalesCore.Extentions;
using OnlineSalesCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesCore.Queries
{
    public class CaseListQuery : ListQuery<OnlineOrder, CaseDTO>
    {
        private readonly ILogger _logger;

        public CaseListQuery(IContextAwareService service, ILogger<CaseListQuery> logger) : base(service)
        {
            _logger = logger;
        }

        protected override IQueryable<OnlineOrder> Filter(IQueryable<OnlineOrder> q, Params param)
        {
            if (q == null || param == null) throw new ArgumentNullException();
            switch (param.Filter)
            {
                case nameof(OnlineOrder.Name):
                    return q.Where(c => c.Name.Contains(param.Contain));
                case nameof(OnlineOrder.Phone):
                    return q.Where(c => c.Phone.Contains(param.Contain));
                case nameof(OnlineOrder.NatId):
                    return q.Where(c => c.NatId.Contains(param.Contain));
                case nameof(OnlineOrder.Induscontract):
                    return q.Where(c => c.Induscontract.Contains(param.Contain));
                default:
                    return q;
            }
        }

        protected override IOrderedQueryable<OnlineOrder> ApplyOrder(IQueryable<OnlineOrder> q, Params param)
        {
            if (q == null || param == null) throw new ArgumentNullException();

            switch (param.OrderBy)
            {
                case nameof(OnlineOrder.Name):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Name);
                    return q.OrderBy(r => r.Name);
                case nameof(OnlineOrder.Received):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Received);
                    return q.OrderBy(r => r.Received);
                case nameof(OnlineOrder.Stage):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Stage.StageNumber);
                    return q.OrderBy(r => r.Stage.StageNumber);
                default:
                    return q.OrderByDescending(r => r.Received);
            }
        }

        public override IQueryable<OnlineOrder> CreateBaseQuery()
        {
            throw new NotImplementedException();
        }

        public override IQueryable<OnlineOrder> CreateBaseQuery(string role)
        {
            if (string.IsNullOrEmpty(role)) throw new ArgumentNullException();
            if (!Enum.TryParse<RoleEnum>(role, true, out var userRole))
            {
                userRole = RoleEnum.UNKNOWN;
            }
            switch (userRole)
            {
                case RoleEnum.UNKNOWN:
                    throw new InvalidOperationException();
                case RoleEnum.CA:
                    return CA_Query();
                case RoleEnum.BDS:
                    return BDS_Query();
                case RoleEnum.ASM:
                    throw new NotImplementedException();
                case RoleEnum.ADMIN:
                    return ADMIN_Query();
                default:
                    throw new InvalidOperationException();
            }
        }
        #region queries
        /// <summary>
        /// All orders assigned to this CA
        /// </summary>
        /// <returns></returns>n
        private IQueryable<OnlineOrder> CA_Query()
        {
            return Service.DbContext.OnlineOrder.Where(o => o.AssignUserId == Service.UserId);
        }
        /// <summary>
        /// All orders assigned to CA and POS under management of this user
        /// </summary>
        /// <returns></returns>
        private IQueryable<OnlineOrder> BDS_Query()
        {
            return  Service.DbContext.OnlineOrder
                .Where(o => o.Pos.User.UserId == Service.UserId || o.AssignUser.Manager.UserId == Service.UserId);
        }
        /// <summary>
        /// Returns all POS
        /// </summary>
        /// <returns></returns>
        private IQueryable<OnlineOrder> ADMIN_Query()
        {
            return Service.DbContext.OnlineOrder;
        }
        #endregion
        protected override async Task<IEnumerable<CaseDTO>> ProjectToOutputAsync(IQueryable<OnlineOrder> q)
        {
            if (q == null) throw new ArgumentNullException();
            var projection = q.Select(p => new CaseDTO()
            {
                OrderId = p.OrderId,
                Name = p.Name,
                NatId = p.NatId,
                Phone = p.Phone,
                Address = p.Address,
                PosCode = p.PosCode,
                Product = p.Product,
                Amount = p.Amount,
                LoanAmount = p.LoanAmount,
                Paid = p.Paid,
                Term = p.Term,
                Received = p.Received,
                Induscontract = p.Induscontract,
                Stage = p.Stage.Stage,
                OrderNumber = p.OrderNumber,
                AssignUser = p.AssignUser != null? new AppUserDTO(p.AssignUser) : null
            });
            return (await projection.ToListAsyncSafe());
        }
    }
}
