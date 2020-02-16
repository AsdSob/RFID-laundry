using System.Security.Principal;

namespace Client.Desktop.ViewModels.Common.Identity
{
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name, string email, string[] roles)
        {
            Name = name;
            Email = email;
            Roles = roles;
        }

        public string Name { get; }
        public string Email { get; }
        public string[] Roles { get; }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }

    public class User
    {
        public User(string username, string email, string[] roles)
        {
            Username = username;
            Email = email;
            Roles = roles;
        }
        public string Username
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string[] Roles
        {
            get;
            set;
        }
    }
}
