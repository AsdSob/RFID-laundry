using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PALMS.Data.Objects;
using PALMS.ViewModels.Common.Services;

namespace PALMS.ViewModels.Services
{
    public class DataService : IDataService
    {
        private readonly IContextFactory _contextFactory;
        private DatabaseState _state;

        public DataService(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task LoadAsync()
        {
            using (var context = new UnitOfWork(_contextFactory.Create()))
            {
                await context.LoadContextAsync();
            }
        }

        public async Task<List<T>> GetAsync<T>() where T : class, IEntityBase
        {
            var result = await RunGetAction<T, IEnumerable<T>>(x => x.GetAllAsync());

            return result?.ToList() ?? new List<T>();
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> match) where T : class, IEntityBase
        {
            var result = await RunGetAction<T, IEnumerable<T>>(x => x.GetAsync(match));

            return result?.ToList() ?? new List<T>();
        }

        public async Task<List<T>> GetAsync<T>(params Expression<Func<T, object>>[] includes) where T : class, IEntityBase
        {
            var result = await RunGetAction<T, IEnumerable<T>>(x => x.GetAllAsync(includes));

            return result?.ToList() ?? new List<T>();
        }

        public async Task AddOrUpdateAsync<T>(T entity) where T : class, IEntityBase
        {
            await RunUpdateAction<T>(x => x.AddOrUpdate(entity));
        }

        public async Task AddOrUpdateAsync<T>(IEnumerable<T> entities) where T : class, IEntityBase
        {
            await RunUpdateAction<T>(x => x.AddOrUpdate(entities));
        }

        public async Task DeleteAsync<T>(T entity) where T: class, IEntityBase
        {
            await RunUpdateAction<T>(x => x.Delete(entity));
        }

        public void SetState(DatabaseState state)
        {
            _state = state;
        }

        private async Task RunUpdateAction<T>(Action<IRepository<T>> action) where T: class, IEntityBase
        {
            if (_state != DatabaseState.Available) return;

            using (var context = new UnitOfWork(_contextFactory.Create()))
            {
                var unitOfWork = context;
                var repository = await Task.Factory.StartNew(() => unitOfWork.GetRepository<T>());

                action(repository);

                await unitOfWork.SaveChangesAsync("admin");

                unitOfWork.Detach();
            }
        }

        private async Task RunUpdateAction<T>(Func<IRepository<T>, Task> action) where T : class, IEntityBase
        {
            if (_state != DatabaseState.Available) return;

            using (var context = new UnitOfWork(_contextFactory.Create()))
            {
                var unitOfWork = context;
                var repository = await Task.Factory.StartNew(() => unitOfWork.GetRepository<T>());

                await action(repository);

                await unitOfWork.SaveChangesAsync("admin");

                unitOfWork.Detach();
            }
        }

        private async Task<TResult> RunGetAction<T, TResult>(Func<IRepository<T>, Task<TResult>> action) where T : class, IEntityBase
        {
            if (_state != DatabaseState.Available) return default(TResult);

            using (var context = new UnitOfWork(_contextFactory.Create()))
            {
                return await action(context.GetRepository<T>());
            }
        }
    }
}