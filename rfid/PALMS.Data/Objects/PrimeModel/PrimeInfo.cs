using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.ClientModel
{
    public class PrimeInfo : NameEntity
    {
        public string TRNNumber { get; set; }
        public double VAT { get; set; }
        public string Address { get; set; }
        public byte[] Logo { get; set; }
    }
}
