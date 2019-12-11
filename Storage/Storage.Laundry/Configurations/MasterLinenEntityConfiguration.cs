using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    class MasterLinenEntityConfiguration : EntityTypeConfigurationBase<MasterLinenEntity>
    {
        public override void Configure(EntityTypeBuilder<MasterLinenEntity> builder)
        {
            builder.ToTable("masterLinen");

            builder.Property(x => x.Name).HasColumnName("name");
        }
    }
}
