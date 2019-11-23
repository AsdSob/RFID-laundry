using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects
{
    public class ClientInfo : Entity
    {
        public string Address { get; set; }
        public int CityId { get; set; }
        public int ClientId { get; set; }
    }
}
