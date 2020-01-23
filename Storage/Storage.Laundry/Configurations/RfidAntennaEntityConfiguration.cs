using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class RfidAntennaEntityConfiguration : EntityTypeConfigurationBase<RfidAntennaEntity>
    {
        public override void Configure(EntityTypeBuilder<RfidAntennaEntity> builder)
        {
            builder.ToTable("rfidAntenna");

            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.AntennaNumb).HasColumnName("antennaNumb");
            builder.Property(x => x.RxSensitivity).HasColumnName("rxSensitivity");
            builder.Property(x => x.TxPower).HasColumnName("txPower");
            builder.Property(x => x.RfidReaderId).HasColumnName("rfidReaderId");

            builder.HasOne(x => x.RfidReaderEntity).WithMany(x => x.RfidAntennaEntities).HasForeignKey(x => x.RfidReaderId);
        }
    }
}
