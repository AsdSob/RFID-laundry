using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace PALMS.ViewModels.Services
{
    public static class ContextExtensions
    {
        public static IQueryable<T> Includes<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));
            }

            return query;
        }
    }
}