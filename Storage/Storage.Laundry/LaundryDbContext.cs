using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry
{
    public class LaundryDbContext : DbContext
    {
        public LaundryDbContext()
        {

        }

        public LaundryDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=laundry_test;User Id=postgres;Password=2235542;Timeout=100;Command Timeout=300;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // load all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LaundryDbContext).Assembly);
        }

        public override int SaveChanges()
        {
            foreach (var entityEntry in ChangeTracker.Entries().Where(x => x.State == EntityState.Added))
            {
                var entity = entityEntry.Entity as EntityBase;
                if (entity == null) continue;

                entity.CreatedDateUtc = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }
    }
}
