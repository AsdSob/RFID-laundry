using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class LaundryEntityConfiguration : EntityTypeConfigurationBase<LaundryEntity>
    {
        public override void Configure(EntityTypeBuilder<LaundryEntity> builder)
        {
            builder.ToTable("laundry");

            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.Description).HasColumnName("description");
        }
    }
}
