using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Configurations.Abstract
{
    public abstract class EntityTypeConfigurationBase<T> : IEntityTypeConfiguration<T> where T: EntityBase
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id).HasName("pk_laundry"); //TODO: need this name?

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.CreatedDateUtc).HasColumnName("created_date_utc");
        }
    }
}
