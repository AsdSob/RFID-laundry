using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class DepartmentEntityConfiguration : EntityTypeConfigurationBase<DepartmentEntity>
    {
        public override void Configure(EntityTypeBuilder<DepartmentEntity> builder)
        {
            builder.ToTable("department");

            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.ClientId).HasColumnName("clientId");
            builder.Property(x => x.DepartmentTypeId).HasColumnName("departmentTypeId");

            builder.HasOne(x => x.ClientEntity).WithMany(x => x.DepartmentEntities).HasForeignKey(x => x.ClientId);
        }
    }
}
