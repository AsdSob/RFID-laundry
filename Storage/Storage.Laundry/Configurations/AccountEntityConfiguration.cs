using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Configurations.Abstract;
using Storage.Laundry.Models;

namespace Storage.Laundry.Configurations
{
    public class AccountEntityConfiguration : EntityTypeConfigurationBase<AccountEntity>
    {
        public override void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.ToTable("account");

            builder.Property(x => x.UserName).HasColumnName("name").IsRequired();
            builder.Property(x => x.Login).HasColumnName("login").IsRequired();
            builder.Property(x => x.Password).HasColumnName("password").IsRequired();
            builder.Property(x => x.Email).HasColumnName("email");
        }
    }
}