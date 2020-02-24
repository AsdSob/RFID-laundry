using Client.Desktop.ViewModels.Common.Identity;

namespace Client.Desktop.ViewModels.Common.Services
{
    public interface IAuthenticationService
    {
        User AuthenticateUser(string username, string password);
        string GetSecretPassword(string clearPassword);
        bool Verify(string clearPassword, string secretPassword);
    }
}