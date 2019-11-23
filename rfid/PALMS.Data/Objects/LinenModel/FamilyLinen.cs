using System.Collections.Generic;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.LinenModel
{
    public class FamilyLinen : NameEntity
    {
        public virtual ICollection<MasterLinen> MasterLinens { get; set; }
        public virtual ICollection<DepartmentContract> DepartmentContracts { get; set; }

    }
}
