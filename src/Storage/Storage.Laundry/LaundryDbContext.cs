using Microsoft.EntityFrameworkCore;

namespace Storage.Laundry
{
    public class LaundryDbContext : DbContext
    {
        public LaundryDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // load all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LaundryDbContext).Assembly);
        }
    }
}
