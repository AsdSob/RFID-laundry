using System.Data.Entity;
using System.Linq;
using PALMS.Data.Objects;
using PALMS.Data.Objects.Audit;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.InvoiceModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Data.Objects.Payment;
using PALMS.Data.Objects.Received_data;
using Z.EntityFramework.Plus;

namespace PALMS.Data.Services
{
    public class DataContext : DbContext
    {
        static DataContext()
        {
            Database.SetInitializer(new ContextInitializer());

            InitAudit();
        }

        public DbSet<CustomAuditEntry> AuditEntries { get; set; }
        public DbSet<CustomAuditEntryProperty> AuditEntryProperties { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<ClientInfo> ClientInfos { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<DepartmentContract> DepartmentContracts { get; set; }

        public DbSet<TaxAndFees> TaxAndFeeses { get; set; }

        public DbSet<FamilyLinen> FamilyLinens { get; set; }

        public DbSet<GroupLinen> GroupLinens { get; set; }

        public DbSet<LinenType> LinenTypes { get; set; }

        public DbSet<MasterLinen> MasterLinens { get; set; }

        public DbSet<LinenList> LinenLists { get; set; }

        public DbSet<LeasingLinen> LeasingLinens { get; set; }

        public DbSet<TpsRecord> TpsRecordses { get; set; }

        public DbSet<NoteHeader> NoteHeaders { get; set; }

        public DbSet<NoteRow> NoteRowses { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<ExtraCharge> ExtraCharges { get; set; }

        public DbSet<PrimeInfo> PrimeInfos { get; set; }

        public DataContext(string connectionString) : base(connectionString)
        {
            
        }

        public DataContext() : base("name=PrimeConnection")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuditEntry>().ToTable("Audit");
            modelBuilder.Entity<AuditEntryProperty>().ToTable("AuditProperties");

            modelBuilder.Entity<Client>()
                .HasMany(x => x.Departments)
                .WithRequired(x => x.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NoteHeader>()
                .HasMany(x => x.NoteRows)
                .WithRequired(x => x.NoteHeader)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(x => x.TaxAndFees)
                .WithRequired(x => x.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(x => x.Invoices)
                .WithRequired(x => x.Client)
                .WillCascadeOnDelete(false);
        }

        private static void InitAudit()
        {
            AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
            {
                var auditEntries = audit.Entries.Cast<CustomAuditEntry>().Select(x =>
                {
                    //correction entityID because when adding row
                    //AuditEntryFactory don't have access to auto create id 
                    x.EntityId = ((IAuditable)x.Entity).Id;

                    if (x.Entity is INameEntity nameEntity)
                        x.Description = nameEntity.Name;

                    return x;
                });

                (context as DataContext)?.AuditEntries.AddRange(auditEntries);
            };


            AuditManager.DefaultConfiguration.Exclude(x => true); // Exclude ALL
            AuditManager.DefaultConfiguration.Include<IAuditable>(); // Need to mark entities you want to audit with IAuditable
            AuditManager.DefaultConfiguration.ExcludeProperty<EntityBase>(x => x.RowVersion); // Do not track changes of RowVersion
            AuditManager.DefaultConfiguration.SoftDeleted<EntityBase>(x => x.DeletedDate != null);

            AuditManager.DefaultConfiguration.AuditEntryFactory = args =>
            {
                var id = ((IAuditable) args.ObjectStateEntry.Entity).Id;
                return new CustomAuditEntry {EntityId = id};
            };

            AuditManager.DefaultConfiguration.AuditEntryPropertyFactory = args =>
            {
                var id = ((IAuditable) args.ObjectStateEntry.Entity).Id;
                var type = args.ObjectStateEntry.Entity.GetType();
                if (type.BaseType != typeof(EntityBase)) type = type.BaseType;
                return new CustomAuditEntryProperty
                {
                    EntityId = id,
                    EntityTypeName = type?.Name
                };
            };
        }
    }
}
