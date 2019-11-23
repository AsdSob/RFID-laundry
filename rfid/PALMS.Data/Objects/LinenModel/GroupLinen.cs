using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.LinenModel
{
    public class GroupLinen : NameEntity
    {
        public string Description { get; set; }

        public virtual ICollection<MasterLinen> MasterLinens { get; set; }
    }
}
