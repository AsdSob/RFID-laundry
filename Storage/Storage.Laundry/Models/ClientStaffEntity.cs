using System.Collections.Generic;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class ClientStaffEntity:EntityBase
    {
        public string StaffId { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public virtual DepartmentEntity DepartmentEntity { get; set; }
        public virtual ICollection<ClientLinenEntity> ClientLinenEntities { get; set; }
    }
}
