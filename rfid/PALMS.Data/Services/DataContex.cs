using System.Data.Entity;
using System.Linq;
using PALMS.Data.Objects;
using PALMS.Data.Objects.EntityModel;
using Z.EntityFramework.Plus;

namespace PALMS.Data.Services
{
    public class DataContext : DbContext
    {
        static DataContext()
        {
            Database.SetInitializer(new ContextInitializer());

        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientInfo> ClientInfos { get; set; }
        public DbSet<ClientLinen> ClientLinens { get; set; }
        public DbSet<ClientStaff> ClientStaffs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<MasterLinen> MasterLinens { get; set; }


        public DataContext(string connectionString) : base(connectionString)
        {
            
        }

        public DataContext() : base("name=PrimeConnection")
        {
            
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<AuditEntry>().ToTable("Audit");
        //    modelBuilder.Entity<AuditEntryProperty>().ToTable("AuditProperties");

        //    modelBuilder.Entity<Client>()
        //        .HasMany(x => x.Departments)
        //        .WithRequired(x => x.Client)
        //        .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<NoteHeader>()
        //        .HasMany(x => x.NoteRows)
        //        .WithRequired(x => x.NoteHeader)
        //        .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<Client>()
        //        .HasMany(x => x.TaxAndFees)
        //        .WithRequired(x => x.Client)
        //        .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<Client>()
        //        .HasMany(x => x.Invoices)
        //        .WithRequired(x => x.Client)
        //        .WillCascadeOnDelete(false);
        //}

    }
}
