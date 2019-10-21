using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PALMS.Data.Objects;

namespace PALMS.ViewModels.Common.Services
{
    public interface IDataService
    {
        Task LoadAsync();
        Task<List<T>> GetAsync<T>() where T : class, IEntityBase;
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> match) where T : class, IEntityBase;
        Task<List<T>> GetAsync<T>(params Expression<Func<T, object>>[] includes) where T : class, IEntityBase;
        Task AddOrUpdateAsync<T>(T entity) where T : class, IEntityBase;
        Task AddOrUpdateAsync<T>(IEnumerable<T> entities) where T : class, IEntityBase;
        Task DeleteAsync<T>(T entity) where T : class, IEntityBase;
        void SetState(DatabaseState state);
    }
}