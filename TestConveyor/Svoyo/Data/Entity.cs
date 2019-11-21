using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConveyor.Svoyo.Data
{
    public abstract class Entity : EntityBase, IEntity
    {
        //[Key]
        public int Id { get; set; }

        //[NotMapped]
        public virtual bool IsNew => Id <= 0;

        public override bool Equals(IEntityBase entityBase)
        {
            var entity = entityBase as IEntity;
            if (entity?.GetType() != GetType())
                return false;

            return entity.Id == Id;
        }
    }
}
