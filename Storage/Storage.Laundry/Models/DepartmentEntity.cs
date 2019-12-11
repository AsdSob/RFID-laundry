using System.Collections.Generic;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class DepartmentEntity : EntityBase
    {
        public string Name { get; set; }
        public int ClientId { get; set; }
        public int DepartmentTypeId { get; set; }

        public virtual ClientEntity ClientEntity { get; set; }
        public virtual ICollection<ClientLinenEntity> ClientLinenEntities { get; set; }
        public virtual ICollection<ClientStaffEntity> ClientStaffEntities { get; set; }

    }
}
