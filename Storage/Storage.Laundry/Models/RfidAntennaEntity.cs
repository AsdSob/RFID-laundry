using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class RfidAntennaEntity : EntityBase
    {
        public string Name { get; set; }

        public int AntennaNumb { get; set; }
        public int RfidReaderId { get; set; }

        public double RxSensitivity { get; set; }
        public double TxPower { get; set; }

        public virtual RfidReaderEntity RfidReaderEntity { get; set; }

    }
}
