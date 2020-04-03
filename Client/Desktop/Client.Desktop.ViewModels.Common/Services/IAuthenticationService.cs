using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Identity;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IAuthenticationService
    {
        Task<User> AuthenticateUserAsync(string username, string password, bool rememberMe = false);
        Task<User> AuthenticateLastUserAsync();
        string GetSecretPassword(string clearPassword);
        bool Verify(string clearPassword, string secretPassword);
    }
}