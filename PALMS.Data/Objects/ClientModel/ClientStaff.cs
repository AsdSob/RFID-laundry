using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class ClientStaff : NameEntity
    {
        public string StaffId { get; set; }
        public int DepartmentId { get; set; }
        public int PackingValue { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<ClientLinen> ClientLinens { get; set; }
    }
}
