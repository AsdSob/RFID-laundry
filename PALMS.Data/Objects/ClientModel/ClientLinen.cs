using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class ClientLinen : Entity
    {
        public int MasterLinenId { get; set; }
        public int DepartmentId { get; set; }
        public int? StaffId { get; set; }
        public string Tag { get; set; }
        public int PackingValue { get; set; }
        public int StatusId { get; set; }

        public virtual ClientStaff  ClientStaff{ get; set; }
        public virtual Department Department { get; set; }
        public virtual MasterLinen MasterLinen { get; set; }
    }
}
