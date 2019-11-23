using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects
{
    public class Client : NameEntity
    {
        public int ParentId { get; set; }
        public string ShortName { get; set; }
        public bool Active { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
    }
}
