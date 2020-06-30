using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Services
{
    public abstract class BaseService
    {
        private readonly IDbContextFactory _contextFactory;

        protected BaseService(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;

        }

        public async Task<ICollection<T>> GetAllAsync<T>() where T : class, IEntity<int>
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var entities = await context.Set<T>().ToListAsync();

                return entities;
            }
        }

        public virtual async Task DeleteAsync<T>(T entity) where T : class, IEntity<int>
        {
            if (entity.Id == 0) return;

            await DeleteAsync(new T[] {entity});
        }

        public virtual async Task DeleteAsync<T>(IEnumerable<T> entities) where T : class, IEntity<int>
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                foreach (var entity in entities)
                {
                    context.Remove(entity);
                }

                context.SaveChanges();
            }
        }

        public virtual async Task AddOrUpdateAsync<T>(T entity) where T : class, IEntity<int>
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                if (entity.Id == 0)
                {
                    context.Attach(entity);
                }
                else
                {
                    context.Update(entity);
                }

                context.SaveChanges();
            }
        }
        
        public async Task AddOrUpdateAsync<T>(ICollection<T> entity) where T : class, IEntity<int>
        {
            using (var context = await _contextFactory.CreateAsync())
            {
                var newEntities = entity.Where(x => x.Id == 0);
                var updateEntities = entity.Where(x => x.Id != 0);

                if (newEntities.Any())
                {
                    context.AttachRange(newEntities);
                }

                if (updateEntities.Any())
                {
                    context.UpdateRange(updateEntities);
                }

                context.SaveChanges();
            }
        }
    }
}