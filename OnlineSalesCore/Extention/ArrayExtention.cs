using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesCore.Extention
{
    public static class ArrayExcention
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(this IQueryable<TSource> source)
        {
            //https://expertcodeblog.wordpress.com/2018/02/19/net-core-2-0-resolve-error-the-source-iqueryable-doesnt-implement-iasyncenumerable/
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source is IAsyncEnumerable<TSource>)
                return source.ToListAsync();
            return Task.FromResult(source.ToList());
            
        }
    }
}
