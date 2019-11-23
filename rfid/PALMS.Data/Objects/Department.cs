using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects
{
    public class Department: NameEntity
    {
        public int ParentId { get; set; }
        public int ClientId { get; set; }
        public int DepartmentTypeId { get; set; }
    }
}
