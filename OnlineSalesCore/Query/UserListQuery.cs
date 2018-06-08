using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.ApiParameter;
using OnlineSalesCore.Const;
using OnlineSalesCore.DTO;
using OnlineSalesCore.EFModel;
using OnlineSalesCore.Extention;
using OnlineSalesCore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesCore.Query
{
    public class UserListQuery : ListQuery<AppUser, AppUserDTO>
    {
        private readonly ILogger<UserListQuery> _logger;

        public UserListQuery(IService service, ILogger<UserListQuery> logger) : base(service)
        {
            if (service == null) throw new ArgumentNullException();
            _logger = logger;
        }

        protected override IQueryable<AppUser> Filter(IQueryable<AppUser> q, ListingParams param)
        {
            if (q == null || param == null) throw new ArgumentNullException();
            switch (param.Filter)
            {
                case nameof(AppUser.Name):
                    return q.Where(c => c.Name.Contains(param.Contain));
                case nameof(AppUser.Username):
                    return q.Where(c => c.Username.Contains(param.Contain));
                case nameof(AppUser.Hr):
                    return q.Where(c => c.Hr.Contains(param.Contain));
                case nameof(AppUser.Phone):
                    return q.Where(c => c.Phone.Contains(param.Contain) || c.Phone2.Contains(param.Contain));
                case "Manager":
                    return q.Where(c => c.Manager.Username.Contains(param.Contain));
                case "":
                    return q;
                default:
                    return q;
            }
        }

        protected override IOrderedQueryable<AppUser> ApplyOrder(IQueryable<AppUser> q, ListingParams param)
        {
            if (q == null || param == null) throw new ArgumentNullException();

            switch (param.OrderBy)
            {
                case nameof(AppUser.Name):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Name);
                    return q.OrderBy(r => r.Name);
                case nameof(AppUser.Username):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Username);
                    return q.OrderBy(r => r.Username);
                case nameof(AppUser.Active):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Active);
                    return q.OrderBy(r => r.Active);
                case nameof(AppUser.Hr):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Hr);
                    return q.OrderBy(r => r.Hr);
                case nameof(AppUser.Role):
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Role.Name);
                    return q.OrderBy(r => r.Role.Name);
                case "Manager":
                    if (!param.Asc)
                        return q.OrderByDescending(r => r.Manager.Username);
                    return q.OrderBy(r => r.Manager.Username);
                default:
                    _logger.LogDebug($"Unknown <{nameof(param.OrderBy)}> value: {param.OrderBy}");
                    return q.OrderByDescending(r => r.Name);
            }
        }

        public override IQueryable<AppUser> CreateBaseQuery()
        {
            // return Service.DbContext.AppUser;
            throw new NotImplementedException();
        }

        public override IQueryable<AppUser> CreateBaseQuery(string role)
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
                    throw new InvalidOperationException();
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
        /// <summary>
        /// Returns all users under this user management
        /// </summary>
        /// <returns></returns>
        private IQueryable<AppUser> BDS_Query()
        {
            return Service.DbContext.AppUser
                .Where(u => u.ManagerId == Service.UserId)
                .Include(u => u.Manager);
        }
        /// <summary>
        /// Returns all users
        /// </summary>
        /// <returns></returns>
        private IQueryable<AppUser> ADMIN_Query()
        {
            return Service.DbContext.AppUser;
        }
        protected override async Task<IEnumerable<AppUserDTO>> ProjectToOutputAsync(IQueryable<AppUser> q)
        {
            if (q == null) throw new ArgumentNullException();
            var projection = q.Select(p => new AppUserDTO(p) {
                Manager = p.Manager == null? null : new AppUserDTO(p.Manager),
                Role = p.Role.Name });
            return (await projection.ToListAsyncSafe());
        }
    }
}
