using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects
{
    public class ClientStaff : NameEntity
    {
        public string StaffId { get; set; }
        public int DepartmentId { get; set; }
    }
}
