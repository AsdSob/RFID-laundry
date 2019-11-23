using System.Collections.Generic;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Data.Objects.Received_data;

namespace PALMS.Data.Objects.LinenModel
{
    public class LinenList : Entity
    {
        public int ClientId { get; set; }

        public int DepartmentId { get; set; }

        public int MasterLinenId { get; set; }
        public bool Active { get; set; }
        public int Weight { get; set; }
        public int UnitId { get; set; }
        public double Laundry { get; set; }
        public double Pressing { get; set; }
        public double DryCleaning { get; set; }

        public virtual Client Client { get; set; }
        public virtual Department Department { get; set; }
        public virtual MasterLinen MasterLinen { get; set; }
        public virtual ICollection<TpsRecord> TpsRecords { get; set; }
        public virtual ICollection<NoteRow> NoteRows { get; set; }
        public virtual ICollection<LeasingLinen> LeasingLinens { get; set; }

    }
}
