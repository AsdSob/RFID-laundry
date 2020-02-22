using System.Collections.Generic;
using System.Threading.Tasks;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IBaseService
    {
        Task<ICollection<T>> GetAllAsync<T>() where T : class, IEntity<int>;
        Task AddOrUpdateAsync<T>(T entity) where T : class, IEntity<int>;
        Task DeleteAsync<T>(T entity) where T : class, IEntity<int>;
        Task DeleteAsync<T>(IEnumerable<T> entities) where T : class, IEntity<int>;
    }
}