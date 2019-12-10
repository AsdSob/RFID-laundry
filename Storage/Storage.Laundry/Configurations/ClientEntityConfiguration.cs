using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class ClientEntityConfiguration: EntityTypeConfigurationBase<ClientEntity>
    {
        public override void Configure(EntityTypeBuilder<ClientEntity> builder)
        {
            builder.ToTable("client");

            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.Active).HasColumnName("active");
            builder.Property(x => x.Address).HasColumnName("address");
            builder.Property(x => x.CityId).HasColumnName("cityId");
            builder.Property(x => x.ParentId).HasColumnName("parentId");
            builder.Property(x => x.ShortName).HasColumnName("shortName");
        }
    }
}
