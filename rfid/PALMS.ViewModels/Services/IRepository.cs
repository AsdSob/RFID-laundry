using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PALMS.Data.Objects;

namespace PALMS.ViewModels.Services
{
    public interface IRepository<T> where T : class, IEntityBase
    {
        Task<T> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> match);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        void Delete(T entity);
        void AddOrUpdate(T entity);
        void AddOrUpdate(IEnumerable<T> entities);
    }
}
