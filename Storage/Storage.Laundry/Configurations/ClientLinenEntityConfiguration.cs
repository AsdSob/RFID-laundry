using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class ClientLinenEntityConfiguration : EntityTypeConfigurationBase<ClientLinenEntity>
    {
        public override void Configure(EntityTypeBuilder<ClientLinenEntity> builder)
        {
            builder.ToTable("clientLinen");

            builder.Property(x => x.ClientId).HasColumnName("clientId");
            builder.Property(x => x.DepartmentId).HasColumnName("departmentId");
            builder.Property(x => x.MasterLinenId).HasColumnName("masterLinenId");
            builder.Property(x => x.RfidTag).HasColumnName("rfidTag");
            builder.Property(x => x.StaffId).HasColumnName("staffId");
            builder.Property(x => x.StatusId).HasColumnName("statusId");
            builder.Property(x => x.PackingValue).HasColumnName("packingValue");

            builder.HasOne(x => x.ClientEntity).WithMany(x => x.ClientLinenEntities).HasForeignKey(x=> x.ClientId);
            builder.HasOne(x => x.DepartmentEntity).WithMany(x => x.ClientLinenEntities).HasForeignKey(x=> x.DepartmentId);
            builder.HasOne(x => x.MasterLinenEntity).WithMany(x => x.ClientLinenEntities).HasForeignKey(x=> x.MasterLinenId);
            builder.HasOne(x => x.ClientStaffEntity).WithMany(x => x.ClientLinenEntities).HasForeignKey(x=> x.StaffId);

        }
    }
}
