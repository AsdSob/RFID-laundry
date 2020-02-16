using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Client.Desktop.ViewModels.Common.Identity;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.Views.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private class InternalUserData
        {
            public string Username { get; }

            public string Email { get; }

            public string HashedPassword { get; }

            public string[] Roles { get; }

            public InternalUserData(string username, string email, string hashedPassword, string[] roles)
            {
                Username = username;
                Email = email;
                HashedPassword = hashedPassword;
                Roles = roles;
            }
        }

        private static readonly List<InternalUserData> _users = new List<InternalUserData>()
        {
            new InternalUserData("Mark", "mark@company.com",
                "MB5PYIsbI2YzCUe34Q5ZU2VferIoI4Ttd+ydolWV0OE=", new[] { Roles.Administrator }),
            new InternalUserData("John", "john@company.com",
                "hMaLizwzOQ5LeOnMuj+C6W75Zl5CXXYbwDSHWW9ZOXc=", new string[] { })
        };

        public User AuthenticateUser(string username, string clearTextPassword)
        {
            InternalUserData userData = _users.FirstOrDefault(u => u.Username.Equals(username)
                                                                   && u.HashedPassword.Equals(CalculateHash(clearTextPassword, u.Username)));
            if (userData == null)
                throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");

            return new User(userData.Username, userData.Email, userData.Roles);
        }

        private string CalculateHash(string clearTextPassword, string salt)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            // Use the hash algorithm to calculate the hash
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // Return the hash as a base64 encoded string to be compared to the stored password
            return Convert.ToBase64String(hash);
        }
    }
}