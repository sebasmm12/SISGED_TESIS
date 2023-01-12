using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SISGED.Client.Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace SISGED.Client.Services.Repositories
{
    public class LoginRepository : AuthenticationStateProvider, ILoginRepository
    {
        [Inject]
        private HttpClient httpClient { get; set; } = default!;
        [Inject]
        private ILocalStorageRepository LocalStorageRepository { get; set; } = default!;

        private static readonly string tokenKey = "TOKENKEY";

        private AuthenticationState anonymous => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await LocalStorageRepository.GetFromLocalStorage(tokenKey);
            if (string.IsNullOrEmpty(token))
            {
                return anonymous;
            }
            return BuildAuthenticationState(token);
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            var claims = ParseToken(token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        private IEnumerable<Claim> ParseToken(string token)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var deserializedToken = jwtSecurityTokenHandler.ReadJwtToken(token);
            return deserializedToken.Claims;
        }
        public async Task Login(string token)
        {
            await LocalStorageRepository.SetInLocalStorage(tokenKey, token);
            var authState = BuildAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await LocalStorageRepository.RemoveItem(tokenKey);
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
        }
    }
}
