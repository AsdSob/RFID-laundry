using System.Collections.Generic;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class MasterLinen : NameEntity
    {
        public int PackingValue { get; set; }

        public virtual ICollection<ClientLinen> ClientLinens { get; set; }
    }
}
