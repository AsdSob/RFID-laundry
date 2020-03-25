using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class AccountEntity : EntityBase
    {
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }

        public virtual AccountDetailsEntity AccountDetailsEntity { get; set; }

    }
}
