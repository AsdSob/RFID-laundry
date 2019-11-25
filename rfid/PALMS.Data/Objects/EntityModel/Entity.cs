using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PALMS.Data.Objects.ClientModel;

namespace PALMS.Data.Objects.EntityModel
{
    public abstract class Entity : EntityBase, IEntity, IAuditable
    {
        [Key]
        public int Id { get; set; }

        [NotMapped]
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