using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConveyor.Svoyo.Data
{
    public abstract class EntityBase : IEntityBase, IEquatable<IEntityBase>
    {
        [Obsolete("use audit")]
        //[NotMapped]
        //[Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        //[Column(TypeName = "datetime2")]
        public DateTime? DeletedDate { get; set; }

        //[Timestamp]
        public virtual byte[] RowVersion { get; set; }

        public abstract bool Equals(IEntityBase entityBase);
    }
}
