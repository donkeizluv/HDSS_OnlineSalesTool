using OnlineSalesTool.ApiParameter;
using OnlineSalesTool.EFModel;
using OnlineSalesTool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Query
{
    /// <summary>
    /// Generic listing of items
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class ListQuery<TSource, TOutput>
    {
        protected readonly IService Service;
        protected ListQuery(IService repo)
        {
            Service = repo ?? throw new ArgumentNullException();
        }
        protected abstract IQueryable<TSource> Filter(IQueryable<TSource> q, ListingParams param);
        protected abstract IOrderedQueryable<TSource> ApplyOrder(IQueryable<TSource> q, ListingParams param);
        protected abstract Task<IEnumerable<TOutput>> ProjectToOutputAsync(IQueryable<TSource> q);
        protected virtual IQueryable<TSource> ApplyPaging(IQueryable<TSource> q, ListingParams param)
        {
            if (q == null || param == null) throw new ArgumentNullException();
            return q.Skip((param.Page - 1) * param.ItemPerPage).Take(param.ItemPerPage);
        }
        protected virtual IQueryable<TSource> TryApplyFilter(IQueryable<TSource> q, ListingParams param)
        {
            if (q == null || param == null) throw new ArgumentNullException();
            if (!string.IsNullOrEmpty(param.Filter) && !string.IsNullOrEmpty(param.Contain))
            {
                q = Filter(q, param);
            }
            return q;
        }

        public abstract IQueryable<TSource> CreateBaseQuery();
        public abstract IQueryable<TSource> CreateBaseQuery(string role);
        /// <summary>
        /// Applies filter, order, paging
        /// </summary>
        /// <param name="baseQuery"></param>
        /// <param name="param"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async virtual Task<(IEnumerable<TOutput>, int total)> ApplyParameters(IQueryable<TSource> baseQuery, ListingParams param)
        {
            if (baseQuery == null || param == null) throw new ArgumentNullException();
            var query = baseQuery;
            query = TryApplyFilter(query, param);
            int total = query.Count();
            query = ApplyOrder(query, param);
            query = ApplyPaging(query, param);
            return (await ProjectToOutputAsync(query), total);
        }
    }
}
