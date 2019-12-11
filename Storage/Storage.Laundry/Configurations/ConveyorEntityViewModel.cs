using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class ConveyorEntityViewModel : EntityTypeConfigurationBase<ConveyorEntity>
    {
        public override void Configure(EntityTypeBuilder<ConveyorEntity> builder)
        {
            builder.ToTable("conveyor");

            builder.Property(x => x.BeltNumber).HasColumnName("beltNumber");
            builder.Property(x => x.ClientLinenId).HasColumnName("clientLinenId");
            builder.Property(x => x.SlotNumber).HasColumnName("slotNumber");

            builder.HasOne(x => x.ClientLinenEntity).WithMany(x => x.ConveyorEntities).HasForeignKey(x=> x.ClientLinenId);
        }
    }
}
