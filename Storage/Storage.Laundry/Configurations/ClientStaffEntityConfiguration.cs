using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    class ClientStaffEntityConfiguration : EntityTypeConfigurationBase<ClientStaffEntity>
    {
        public override void Configure(EntityTypeBuilder<ClientStaffEntity> builder)
        {
            builder.ToTable("clientStaff");

            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.DepartmentId).HasColumnName("departmentId");
            builder.Property(x => x.StaffId).HasColumnName("staffId");

        }
    }
}
