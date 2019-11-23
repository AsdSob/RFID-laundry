using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class ClientStaffEntity:EntityBase
    {
        public string StaffId { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
    }
}
