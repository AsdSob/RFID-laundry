using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class DepartmentEntity : EntityBase
    {
        public string Name { get; set; }
        public int ParentId { get; set; }
        public int ClientId { get; set; }
        public int DepartmentTypeId { get; set; }
    }
}
