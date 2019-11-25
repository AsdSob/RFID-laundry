using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PALMS.Data.Objects.EntityModel
{
    public abstract class EntityBase : IEntityBase, IEquatable<IEntityBase>
    {
        [Obsolete("use audit")]
        [NotMapped]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DeletedDate { get; set; }

        [Timestamp]
        public virtual byte[] RowVersion { get; set; }

        public abstract bool Equals(IEntityBase entityBase);
    }
}
