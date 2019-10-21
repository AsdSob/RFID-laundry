using PALMS.Data.Objects.EntityModel;

namespace PALMS.Data.Objects.Audit
{
    public class User : NameEntity
    {
        public int UserRoleId { get; set; }

        public string Password { get; set; }
    }
}
