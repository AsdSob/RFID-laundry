using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PALMS.ViewModels.Common.Services;

namespace PALMS.WPFClient.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IAuthSettings _authSettings;
        private readonly ISerializer _serializer;
        private HttpClient _httpClient;

        public AuthService(IAuthSettings authSettings, ISerializer serializer, IUserIdentity userIdentity)
        {
            _userIdentity = userIdentity ?? throw new ArgumentNullException(nameof(userIdentity));
            _authSettings = authSettings ?? throw new ArgumentNullException(nameof(authSettings));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            await Task.Delay(50);

            _userIdentity.UserName = loginRequest.UserName;

            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(_authSettings.BaseUri);
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token");
                _httpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            var requestContent = _serializer.Serialize(loginRequest);
            var content = new StringContent(requestContent,
                Encoding.UTF8,
                "application/json");

            var response = new LoginResponse();

            HttpResponseMessage result = null;

            try
            {
                result = _httpClient.PostAsync(_authSettings.RequestUri, content).Result;
            }
            catch(Exception exception) when (exception is WebException ||
                                          exception is HttpRequestException)
            {
                response.ErrorText = exception.Message;
                return response;
            }
            catch(Exception exception) when (exception is AggregateException)
            {
                var errorText = new StringBuilder();
                foreach (var ex in exception.FromHierarchy(e => e.InnerException)
                                            .Where(x => x.GetType() == typeof(WebException)))
                {
                    errorText.Append(ex.Message);
                }

                response.ErrorText = errorText.ToString();
                return response;
            }

            response.StatusCode = (int)result.StatusCode;

            string resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                response.AuthResponse = _serializer.Deserialize<AuthResponse>(resultContent);
            }
            else
            {
                if (result.StatusCode == HttpStatusCode.BadRequest)
                    response.ErrorText = "Bad request";
                else if (result.StatusCode == HttpStatusCode.InternalServerError)
                    response.ErrorText = "Internal server error";
                else if (result.StatusCode == HttpStatusCode.RequestTimeout)
                    response.ErrorText = "Request timeout";
                else if (result.StatusCode == HttpStatusCode.Unauthorized)
                    response.ErrorText = "Bad login or password";

                response.Description = resultContent;
            }

            return response;
        }

        public object Logout()
        {
            return null;
        }
    }

    public class AuthSettings : IAuthSettings
    {
        public string BaseUri { get; set; }
        public string RequestUri { get; set; }
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
