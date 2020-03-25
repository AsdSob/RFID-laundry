using System.Collections.Generic;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class RfidReaderEntity : EntityBase
    {
        public string Name { get; set; }
        public string ReaderIp { get; set; }
        public int ReaderPort { get; set; }
        public int TagPopulation { get; set; }

        public virtual ICollection<RfidAntennaEntity> RfidAntennaEntities { get; set; }
        public virtual ICollection<AccountDetailsEntity> AccountDetailsEntities { get; set; }

    }
}
