using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PALMS.ViewModels.Common.Services
{
    public interface IAuthService : IDisposable
    {
        Task<LoginResponse> Login(LoginRequest request);
        object Logout();
    }

    public interface IUserIdentity
    {
        string UserName { get; set; } // TODO: private set, init only once
    }

    public class UserIdentity : IUserIdentity
    {
        public string UserName { get; set; }
    }

    public interface IAuthSettings
    {
        string BaseUri { get; set; }
        string RequestUri { get; set; }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public int StatusCode { get; set; }
        public AuthResponse AuthResponse { get; set; }
        public string Description { get; set; }
        public string ErrorText { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public ICollection<RoleEnum> Roles { get; set; }
    }

    public enum RoleEnum
    {
        None,
        Admin,
        Account,
        Reception,
        Operator,
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AuthRolesAttribute : Attribute
    {
        public ICollection<RoleEnum> Roles { get; set; }

        public AuthRolesAttribute(params RoleEnum[] roles)
        {
            Roles = roles.ToList();
        }
    }
}
