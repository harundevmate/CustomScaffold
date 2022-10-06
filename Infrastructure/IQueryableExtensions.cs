using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> EagerLoadInclude<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes) where T : class
        {
            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            return query;
        }

        public static IQueryable<T> EagerLoadWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>> wheres) where T : class
        {
            if (wheres != null)
                query = query.Where(wheres);

            return query;
        }
    }
}
