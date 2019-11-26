using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class Department : NameEntity
    {
        public int ClientId { get; set; }
        public int? ParentId { get; set; }
        public int DepartmentTypeId { get; set; }


        public virtual ICollection<ClientStaff> ClientStaves { get; set; }
        public virtual ICollection<ClientLinen> ClientLinens { get; set; }
        public virtual Department Parent { get; set; }
        public virtual Client Client { get; set; }
    }
}
