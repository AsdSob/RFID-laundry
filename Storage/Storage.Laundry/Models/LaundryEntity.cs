using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class LaundryEntity : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
