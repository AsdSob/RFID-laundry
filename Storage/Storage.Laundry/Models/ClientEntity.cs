using System;
using System.Collections.Generic;
using System.Text;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class ClientEntity : EntityBase
    {
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool Active { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }

        public virtual ClientEntity Parent { get; set; }
        public virtual ICollection<ClientEntity> ChildEntities { get; set; }
        public virtual ICollection<DepartmentEntity> DepartmentEntities { get; set; }
        public virtual ICollection<ClientLinenEntity> ClientLinenEntities { get; set; }
    }
}
