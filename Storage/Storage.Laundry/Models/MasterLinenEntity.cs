using System.Collections.Generic;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class MasterLinenEntity : EntityBase
    {
        public string Name { get; set; }
        public int PackingValue { get; set; }

        public virtual ICollection<ClientLinenEntity> ClientLinenEntities { get; set; }

    }
}
