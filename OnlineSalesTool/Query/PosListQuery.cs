using Microsoft.EntityFrameworkCore;
using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.AppEnum;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Helper;
using OnlineSalesTool.DTO;
using OnlineSalesTool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OnlineSalesTool.Query
{
    public class PosListQuery : ListQuery<Pos, PosDTO>
    {
        private readonly ILogger<PosListQuery> _logger;

        public PosListQuery(IService repo, ILogger<PosListQuery> logger) : base(repo)
        {
            if (repo == null) throw new ArgumentNullException();
            _logger = logger;
        }

        protected override IQueryable<Pos> Filter(IQueryable<Pos> q, ListingParams param)
        {
            if (q == null || param == null) throw new ArgumentNullException();
            switch (param.Filter)
            {
                case nameof(Pos.PosName):
                    return q.Where(c => c.PosName.Contains(param.Contain));
                case nameof(Pos.PosCode):
                    return q.Where(c => c.PosCode.Contains(param.Contain));
                case nameof(Pos.Phone):
                    return q.Where(c => c.Phone.Contains(param.Contain));
                case "Manager":
                    return q.Where(c => c.User.Username.Contains(param.Contain));
                case "":
                    return q;
                default:
                    return q;
            }
        }

        protected override IOrderedQueryable<Pos> ApplyOrder(IQueryable<Pos> q, ListingParams param)
        {
            if (q == null || param == null) throw new ArgumentNullException();

            switch (param.OrderBy)
            {
                case nameof(Pos.PosName):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.PosName);
                    return q.OrderBy(r => r.PosName);
                case nameof(Pos.PosCode):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.PosCode);
                    return q.OrderBy(r => r.PosCode);
                case "Manager":
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.User.Username);
                    return q.OrderBy(r => r.User.Username);
                default:
                    _logger.LogDebug($"Unknown <{nameof(param.OrderBy)}> value: {param.OrderBy}");
                    return q.OrderByDescending(r => r.PosName);
            }
        }

        public override IQueryable<Pos> CreateBaseQuery()
        {
            throw new NotImplementedException();
        }

        public override IQueryable<Pos> CreateBaseQuery(string role)
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
        /// Returns POS under management of this CA's manager
        /// </summary>
        /// <returns></returns>
        private IQueryable<Pos> CA_Query()
        {
            int mandId = Service.DbContext.AppUser
                .Where(u => u.UserId == Service.UserId)
                .Select(u => u.ManagerId ?? -1)
                .First();
            if (mandId == -1)
            {
                //Invalid
                _logger.LogDebug($"CA UserId: {Service.UserId} doesnt have manager!");
            }
            return Service.DbContext.Pos.Where(p => p.UserId == mandId)
                .Include(p => p.PosShift)
                    .ThenInclude(p => p.Shift);
        }
        /// <summary>
        /// Returns all POS under this user management
        /// </summary>
        /// <returns></returns>
        private IQueryable<Pos> BDS_Query()
        {
            return Service.DbContext.Pos
                .Where(p => p.UserId == Service.UserId)
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
            return Service.DbContext.Pos
                .Include(p => p.User)
                .Include(p => p.PosShift)
                    .ThenInclude(ps => ps.Shift); ;
        }
        #endregion
        protected override async Task<IEnumerable<PosDTO>> ProjectToOutputAsync(IQueryable<Pos> q)
        {
            if (q == null) throw new ArgumentNullException();
            var projection = q.Select(p => new PosDTO(p) { 
                BDS = new AppUserDTO(p.User),
                Shifts = p.PosShift.Select(ps => new ShiftDTO(ps.Shift))
             });
            return (await projection.ToListAsyncSafe());
        }
    }
}
