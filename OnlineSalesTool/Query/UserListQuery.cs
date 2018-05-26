using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.ApiParameter;using OnlineSalesTool.EFModel;
using OnlineSalesTool.Helper;
using OnlineSalesTool.POCO;
using OnlineSalesTool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Query
{
    public class UserListQuery : ListQuery<AppUser, AppUserPOCO>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserListQuery(IService repo) : base(repo)
        {
            if (repo == null) throw new ArgumentNullException();
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
                    _logger.Debug($"Unknown <{nameof(param.OrderBy)}> value: {param.OrderBy}");
                    return q.OrderByDescending(r => r.Name);
            }
        }

        public override IQueryable<AppUser> CreateBaseQuery()
        {
            return Repo.DbContext.AppUser;
        }

        public override IQueryable<AppUser> CreateBaseQuery(string role)
        {
            throw new NotImplementedException();
        }
       
        protected override async Task<IEnumerable<AppUserPOCO>> ProjectToOutputAsync(IQueryable<AppUser> q)
        {
            if (q == null) throw new ArgumentNullException();
            var projection = q.Select(p => new AppUserPOCO(p) {
                Manager = p.Manager == null? null : new AppUserPOCO(p.Manager),
                Role = p.Role.Name });
            return (await projection.ToListAsyncSafe());
        }
    }
}
