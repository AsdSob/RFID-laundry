using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PALMS.Data.Objects;
using PALMS.Data.Services;
using Z.EntityFramework.Plus;

namespace PALMS.ViewModels.Services
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed;
        private readonly DataContext _context;
        private Hashtable _repositories;
        private static List<Type> _types;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }

        public async Task<int> SaveChangesAsync(string userName)
        {
            var audit = new Audit {CreatedBy = userName};

            return await _context.SaveChangesAsync(audit).ConfigureAwait(false);
        }

        public IRepository<T> GetRepository<T>() where T : class, IEntityBase
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var genericRepositoryType = repositoryType.MakeGenericType(typeof(T));

                if (_types == null)
                    _types = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => x.FullName.Contains("PALMS"))
                        .SelectMany(x => x.GetTypes())
                        .ToList();

                var exist = _types.FirstOrDefault(x => genericRepositoryType.IsAssignableFrom(x));
                if (exist != null)
                    genericRepositoryType = exist;

                var repositoryInstance = Activator.CreateInstance(genericRepositoryType, _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }


        public async Task LoadContextAsync()
        {
            // use any actions of context for loading

            await _context.Clients.ToListAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Detach()
        {
            foreach (var dbEntityEntry in _context.ChangeTracker.Entries())
            {
                if (dbEntityEntry.Entity != null)
                  dbEntityEntry.State = EntityState.Detached;
            }
        }
    }
}
