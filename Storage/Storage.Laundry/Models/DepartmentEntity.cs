using System.Collections.Generic;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class DepartmentEntity : EntityBase
    {
        public string Name { get; set; }
        public int ClientId { get; set; }
        public int? DepartmentTypeId { get; set; }
        public int? ParentId { get; set; }

        public virtual DepartmentEntity Parent { get; set; }
        public virtual ICollection<DepartmentEntity> Children { get; set; }

        public virtual ClientEntity ClientEntity { get; set; }
        public virtual ICollection<ClientLinenEntity> ClientLinenEntities { get; set; }
        public virtual StaffDetailsEntity StaffDetailsEntity { get; set; }

    }
}
