using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class Client : NameEntity
    {
        public string ShortName { get; set; }
        public bool Active { get; set; }
        public int? ParentId { get; set; }
        public int? CityId { get; set; }


        public virtual ICollection<Department> Departments { get; set; }

        public virtual Client Parent { get; set; }
    }
}
