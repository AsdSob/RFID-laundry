using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    class StaffDetailsEntityConfiguration : EntityTypeConfigurationBase<StaffDetailsEntity>
    {
        public override void Configure(EntityTypeBuilder<StaffDetailsEntity> builder)
        {
            builder.ToTable("clientStaff");

            builder.Property(x => x.DepartmentId).HasColumnName("departmentId");
            builder.Property(x => x.StaffId).HasColumnName("staffId");
            builder.Property(x => x.PhoneNumber).HasColumnName("phoneNumber");
            builder.Property(x => x.Email).HasColumnName("email");
            builder.Property(x => x.Name).HasColumnName("name");

            builder.HasOne(x => x.DepartmentEntity)
                .WithOne(x => x.StaffDetailsEntity)
                .HasForeignKey<StaffDetailsEntity>(x => x.DepartmentId);

        }
    }
}
