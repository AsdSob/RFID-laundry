using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;
using Storage.Laundry.Models.Abstract;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface ILaundryService
    {
        Task<ICollection<T>> GetAllAsync<T>() where T : class, IEntity<int>;
        Task AddOrUpdate<T>(T entity) where T : class, IEntity<int>;

        Task Delete<T>(T entity) where T : class, IEntity<int>;
    }
    

    public class LaundryService : ILaundryService
    {
        private readonly IDbContextFactory _contextFactory;

        public LaundryService(IDbContextFactory contextFactory)
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

        public async Task Delete<T>(T entity) where T : class, IEntity<int>
        {
            if(entity.Id == 0) return;

            using (var context = await _contextFactory.CreateAsync())
            {
                context.Remove(entity);
                context.SaveChanges();
            }
        }

        public async Task AddOrUpdate<T>(T entity) where T : class, IEntity<int>
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


    }
}

