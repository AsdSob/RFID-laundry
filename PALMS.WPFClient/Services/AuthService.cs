using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PALMS.ViewModels.Common.Services;
using PALMS.Data.Objects.Audit;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.WPFClient.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IDataService _dataService;
        private HttpClient _httpClient;
        public List<User> Users { get; set; }


        public AuthService(IUserIdentity userIdentity, IDataService dataService)
        {
            _userIdentity = userIdentity ?? throw new ArgumentNullException(nameof(userIdentity));
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            await Task.Delay(50);

            _userIdentity.UserName = loginRequest.UserName;
            var response = new LoginResponse();
            Users = await _dataService.GetAsync<User>();

            var loginName = loginRequest.UserName.ToUpper();
            var loginPassword = loginRequest.Password;
            var authResponse = new AuthResponse();

            foreach (var user in Users)
            {
                var userName = user.Name.ToUpper();
                if (userName == loginName && user.Password == loginPassword)
                {
                    authResponse.Roles = new List<RoleEnum>();
                    switch (user.UserRoleId)
                    {
                        case 1:
                            authResponse.Roles.Add(RoleEnum.Admin);
                            break;
                        case 2:
                            authResponse.Roles.Add(RoleEnum.Account);
                            break;
                        case 3:
                            authResponse.Roles.Add(RoleEnum.Supervisor);
                            break;
                        case 4:
                            authResponse.Roles.Add(RoleEnum.Reception);
                            break;
                        case 5:
                            authResponse.Roles.Add(RoleEnum.Operator);
                            break;
                    }

                    response.IsUserExcist = true;
                }
            }

            response.AuthResponse = authResponse;
            if (!response.IsUserExcist)
            {
                response.Description = "Bad login or password";
                response.ErrorText = "Bad login or password";
            }

            return response;
        }

        public object Logout()
        {
            return null;
        }
    }


    /// <summary>
    /// All collection and enumerable methods.
    /// </summary>
    public static class Collections
    {
        /// <summary>
        /// Convert Enumerable in hierarchy format to Enumerable collection.
        /// </summary>
        /// <typeparam name="TSource">Originating source collection type.</typeparam>
        /// <param name="source">Originating source collection type.</param>
        /// <param name="nextItem">Function to retrieve next item in collection.</param>
        /// <param name="canContinue">Boolean function indicating if next item exists.</param>
        /// <returns>The collection from a hierarchical format.</returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        /// <summary>
        /// Recursively enumerates over hierarchy to get collection.
        /// </summary>
        /// <typeparam name="TSource">Originating source collection type.</typeparam>
        /// <param name="source">Originating source collection type.</param>
        /// <param name="nextItem">Function to retrieve next item in collection.</param>
        /// <returns>Single yielded enumerable object.</returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
    }
}
