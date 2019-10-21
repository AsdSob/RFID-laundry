using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.LinenModel
{
    public class MasterLinen : Entity
    {
        public string Name { get; set; }

        public int LinenTypeId { get; set; }

        public int FamilyLinenId { get; set; }

        public int GroupLinenId { get; set; }

        public int? Weight { get; set; }

        public virtual ICollection<LinenList> LinenLists { get; set; }

        public virtual LinenType LinenType { get; set; }
        public virtual FamilyLinen FamilyLinen { get; set; }
        public virtual GroupLinen GroupLinen { get; set; }

    }
}
