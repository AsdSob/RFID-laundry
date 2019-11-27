using System.Data.Entity;
using PALMS.Data.Objects.ClientModel;

namespace PALMS.Data.Services
{
    public class DataContext : DbContext
    {
        static DataContext()
        {
            Database.SetInitializer(new ContextInitializer());

        }


        public DbSet<Client> Clients { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ClientLinen> ClientLinens { get; set; }
        public DbSet<ClientStaff> ClientStaves { get; set; }
        public DbSet<MasterLinen> MasterLinens { get; set; }


        public DataContext(string connectionString) : base(connectionString)
        {
            
        }

        public DataContext() : base("name=PrimeConnection")
        {
            
        }

    }
}
