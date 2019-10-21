using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.LinenModel;
using PALMS.Data.Objects.NoteModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class Department : NameEntity
    {
        public int ClientId { get; set; }
        public int? ParentId { get; set; }
        public bool AllFree { get; set; }
        public bool Labeling { get; set; }
        public int? DepartmentType { get; set; }
        public double Express { get; set; }

        public virtual Client Client { get; set; }

        public virtual Department Parent { get; set; }

        public virtual ICollection<NoteHeader> NoteHeaders { get; set; }
        public virtual ICollection<LinenList> LinenLists { get; set; }
        public virtual ICollection<DepartmentContract> DepartmentContracts { get; set; }

    }
}
