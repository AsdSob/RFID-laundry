using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Client.Desktop.ViewModels.Common.Identity;
using Client.Desktop.ViewModels.Common.Model;
using Client.Desktop.ViewModels.Common.Services;
using Microsoft.EntityFrameworkCore;
using Storage.Core.Abstract;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string LocalDataFileName = "laundry.settings";

        private readonly IDbContextFactory _dbContextFactory;
        private readonly ISettingsManagerProvider _settingsManagerProvider;

        public AuthenticationService(IDbContextFactory dbContextFactory, ISettingsManagerProvider settingsManagerProvider)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _settingsManagerProvider = settingsManagerProvider ?? throw new ArgumentNullException(nameof(settingsManagerProvider));

            using (var context = _dbContextFactory.Create())
            {
                var account = context.Set<AccountEntity>().FirstOrDefault();
                if (account == null)
                {
                    // TODO: use seed dbcontext seed

                    context.Set<AccountEntity>().Add(new AccountEntity
                    {
                        UserName = "Admin",
                        Login = "admin",
                        Password = CalculateHash("test"),
                        Email = "admin@laundry.com",
                        Roles = $"{Roles.Administrator}"
                    });

                    context.SaveChanges();
                }
            }
        }

        public async Task<User> AuthenticateUserAsync(string username, string clearTextPassword, bool rememberMe = false)
        {
            AccountEntity account;

            using (var context = _dbContextFactory.Create())
            {
                var login = username.Trim();
                account = await context.Set<AccountEntity>().FirstOrDefaultAsync(x => x.Login.Equals(login));
                if (account == null || VerifyPassword(clearTextPassword, account.Password) == false)
                    throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");
            }

            if (rememberMe)
            {
                // TODO: use decorator
                var localData = await _settingsManagerProvider.GetAsync<LocalData>(LocalDataFileName);
                if (localData == null)
                    localData = new LocalData();
                if (localData.LastUser == null)
                    localData.LastUser = new UserData();

                localData.LastUser.Login = account.Login;
                localData.LastUser.Password = account.Password;

                await _settingsManagerProvider.SaveAsync(localData, LocalDataFileName);
            }
            

            return new User(account.UserName, account.Email, account.Roles?.Split(',') ?? new string[] { });

        }

        public async Task<User> AuthenticateLastUserAsync()
        {
            var localData = await _settingsManagerProvider.GetAsync<LocalData>(LocalDataFileName);

            if (string.IsNullOrEmpty(localData?.LastUser?.Login) ||
                string.IsNullOrEmpty(localData.LastUser.Password))
            {
                return null;
            }

            using (var context = _dbContextFactory.Create())
            {
                var login = localData.LastUser.Login;
                var password = localData.LastUser.Password;

                var account = await context.Set<AccountEntity>()
                    .FirstOrDefaultAsync(x => x.Login.Equals(login) && x.Password == password);

                if (account == null)
                    throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");

                return new User(account.UserName, account.Email, account.Roles?.Split(',') ?? new string[] { });
            }
        }

        public string GetSecretPassword(string clearPassword)
        {
            return CalculateHash(clearPassword);
        }

        public bool Verify(string clearPassword, string secretPassword)
        {
            return VerifyPassword(clearPassword, secretPassword);
        }

        public async Task LogoutAsync()
        {
            var localData = await _settingsManagerProvider.GetAsync<LocalData>(LocalDataFileName);

            if (localData?.LastUser != null && !string.IsNullOrEmpty(localData.LastUser.Password))
            {
                localData.LastUser = null;

                await _settingsManagerProvider.SaveAsync(localData, LocalDataFileName);
            }
        }

        private string CalculateHash(string clearTextPassword)
        {
            //Store a password hash:
            PasswordHash hash = new PasswordHash(clearTextPassword);
            byte[] hashBytes = hash.ToArray();

            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string clearTextPassword, string password)
        {
            PasswordHash hash = new PasswordHash(Convert.FromBase64String(password));
            return hash.Verify(clearTextPassword);
        }
    }

    public sealed class PasswordHash
    {
        const int SaltSize = 16, HashSize = 20, HashIter = 10000;
        readonly byte[] _salt, _hash;
        public PasswordHash(string password)
        {
            new RNGCryptoServiceProvider().GetBytes(_salt = new byte[SaltSize]);
            _hash = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
        }
        public PasswordHash(byte[] hashBytes)
        {
            Array.Copy(hashBytes, 0, _salt = new byte[SaltSize], 0, SaltSize);
            Array.Copy(hashBytes, SaltSize, _hash = new byte[HashSize], 0, HashSize);
        }
        public PasswordHash(byte[] salt, byte[] hash)
        {
            Array.Copy(salt, 0, _salt = new byte[SaltSize], 0, SaltSize);
            Array.Copy(hash, 0, _hash = new byte[HashSize], 0, HashSize);
        }
        public byte[] ToArray()
        {
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(_salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(_hash, 0, hashBytes, SaltSize, HashSize);
            return hashBytes;
        }
        public byte[] Salt { get { return (byte[])_salt.Clone(); } }
        public byte[] Hash { get { return (byte[])_hash.Clone(); } }
        public bool Verify(string password)
        {
            byte[] test = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
            for (int i = 0; i < HashSize; i++)
                if (test[i] != _hash[i])
                    return false;
            return true;
        }
    }
}