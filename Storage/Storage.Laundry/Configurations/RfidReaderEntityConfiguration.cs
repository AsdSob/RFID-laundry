using System;
using System.Collections.Generic;
using System.Text;
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

            builder.Property(x => x.Antenna1Rx).HasColumnName("antenna1Rx");
            builder.Property(x => x.Antenna1Tx).HasColumnName("antenna1Tx");

            builder.Property(x => x.Antenna2Rx).HasColumnName("antenna2Rx");
            builder.Property(x => x.Antenna2Tx).HasColumnName("antenna2Tx");

            builder.Property(x => x.Antenna3Rx).HasColumnName("antenna3Rx");
            builder.Property(x => x.Antenna3Tx).HasColumnName("antenna3Tx");

            builder.Property(x => x.Antenna4Rx).HasColumnName("antenna4Rx");
            builder.Property(x => x.Antenna4Tx).HasColumnName("antenna4Tx");

        }
    }
}
