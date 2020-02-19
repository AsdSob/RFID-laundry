using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;

namespace Storage.Laundry
{
    public class LaundryDbContextFactory : IDbContextFactory
    {
        private readonly DbContextOptions<LaundryDbContext> _options;

        public LaundryDbContextFactory(IDbConfiguration dbConfiguration)
        {
            if (dbConfiguration == null) throw new ArgumentNullException(nameof(dbConfiguration));
            if (string.IsNullOrEmpty(dbConfiguration.ConnectionString)) throw new ArgumentNullException(nameof(dbConfiguration.ConnectionString));

            var contextOptionsBuilder = new DbContextOptionsBuilder<LaundryDbContext>();

            contextOptionsBuilder.UseNpgsql(dbConfiguration.ConnectionString);

            _options = contextOptionsBuilder.Options;
        }

        public async Task<DbContext> CreateAsync()
        {
            var context = new LaundryDbContext(_options);

            return context;
        }

        public DbContext Create()
        {
            var context = new LaundryDbContext(_options);

            return context;
        }
    }
}