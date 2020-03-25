using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class AccountDetailsEntityConfiguration : EntityTypeConfigurationBase<AccountDetailsEntity>
    {
        public override void Configure(EntityTypeBuilder<AccountDetailsEntity> builder)
        {
            builder.ToTable("accountDetails");

            builder.Property(x => x.AccountId).HasColumnName("accountId").IsRequired();
            builder.Property(x => x.ReaderId).HasColumnName("readerId");

            builder.HasOne(x => x.AccountEntity)
                .WithOne(x => x.AccountDetailsEntity)
                .HasForeignKey<AccountDetailsEntity>(x => x.AccountId);
            builder.HasOne(x => x.RfidReaderEntity)
                .WithMany(x => x.AccountDetailsEntities)
                .HasForeignKey(x => x.ReaderId);

        }
    }
}
