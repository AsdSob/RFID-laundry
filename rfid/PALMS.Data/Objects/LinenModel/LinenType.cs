using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.LinenModel
{
    public class LinenType : NameEntity
    {
        public virtual ICollection<MasterLinen> MasterLinens { get; set; }
    }
}
