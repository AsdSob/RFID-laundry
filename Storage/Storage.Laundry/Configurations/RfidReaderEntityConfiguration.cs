using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class RfidReaderEntityConfiguration : EntityTypeConfigurationBase<RfidReaderEntity>
    {
        public override void Configure(EntityTypeBuilder<RfidReaderEntity> builder)
        {
            builder.ToTable("rfidReader");

            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.ReaderIp).HasColumnName("readerIp");
            builder.Property(x => x.ReaderPort).HasColumnName("readerPort");
            builder.Property(x => x.TagPopulation).HasColumnName("tagPopulation");

        }
    }
}
