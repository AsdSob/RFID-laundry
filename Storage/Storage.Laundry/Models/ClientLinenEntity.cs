using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class ClientLinenEntity: EntityBase
    {
        public int DepartmentId { get; set; }
        public int ClientId { get; set; }
        public int MasterLinenId { get; set; }
        public int? StaffId { get; set; }
        public string RfidTag { get; set; }
        public int StatusId { get; set; }
    }
}
