using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PALMS.Data.Objects;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Services;

namespace PALMS.ViewModels.Services
{
    public class MasterLinenRepository : Repository<MasterLinen>
    {
        public MasterLinenRepository(DataContext context) : base(context)
        {
        }

        /// <summary>
        /// TODO:ПроАнализировать MasterLinenID
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        //public override async Task AddOrUpdateAsync(IEnumerable<MasterLinen> entities)
        //{
        //    var items = entities.ToList();

        //    var familyLinenIds = items.Where(x => x.IsNew).Select(x => x.FamilyLinenId).Distinct().ToList();
        //    if (familyLinenIds.Any())
        //    {
        //        var masterLinens = (await GetAsync(x => familyLinenIds.Contains(x.FamilyLinenId))).ToList();
        //        Dictionary<int, int> masterLinensByFamily = new Dictionary<int, int>();

        //        foreach (var masterLinen in masterLinens)
        //        {
        //            if (!masterLinensByFamily.TryGetValue(masterLinen.FamilyLinenId, out var max))
        //            {
        //                masterLinensByFamily.Add(masterLinen.FamilyLinenId, masterLinen.MasterLinenID);
        //                continue;
        //            }

        //            max = Math.Max(max, masterLinen.MasterLinenID);
        //            masterLinensByFamily[masterLinen.FamilyLinenId] = max;
        //        }

        //        foreach (var item in items)
        //        {
        //            if (!masterLinensByFamily.TryGetValue(item.FamilyLinenId, out var max))
        //            {
        //                item.MasterLinenID = 1;
        //                continue;
        //            }
        //            item.MasterLinenID = ++max;
        //            masterLinensByFamily[item.FamilyLinenId] = max;
        //        }
        //    }

        //    await base.AddOrUpdateAsync(items);
        //}
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(DataContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        protected DbSet<T> GetDbSet<T>() where T: class
        {
            return _context.Set<T>();
        }

        public void AddOrUpdate(TEntity entity)
        {
            _dbSet.AddOrUpdate(entity);
        }

        public virtual void AddOrUpdate(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                AddOrUpdate(entity);
        }

        public virtual void AddOrUpdateAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                AddOrUpdate(entity);
        }

        public async Task<TEntity> GetAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.Where(x => !x.DeletedDate.HasValue)
                               .AsNoTracking()
                               .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _dbSet.Where(match)
                               .Where(x => !x.DeletedDate.HasValue)
                               .AsNoTracking()
                               .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            return await _dbSet.Where(x => !x.DeletedDate.HasValue)
                .Includes(includes)
                .AsNoTracking()
                .ToListAsync();
        }

        public void Delete(TEntity entity)
        {
            entity.DeletedDate = DateTime.UtcNow;
            AddOrUpdate(entity);
        }

        protected virtual IQueryable<TEntity> GetEntityQuery()
        {
            return _context.Set<TEntity>();
        }
    }
}
