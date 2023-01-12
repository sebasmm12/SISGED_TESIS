using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace SISGED.Client.Services.Repositories
{
    public class LoginRepository : AuthenticationStateProvider, ILoginRepository
    {
        private HttpClient httpClient { get; set; } = default!;
        private readonly ILocalStorageRepository _localStorageRepository;

        public LoginRepository(ILocalStorageRepository localStorageRepository)
        {
            _localStorageRepository = localStorageRepository;
        }

        private readonly string tokenKey = "TOKENKEY";

        private AuthenticationState anonymous => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageRepository.GetFromLocalStorage(tokenKey)!;
            if (token is null)
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
            await _localStorageRepository.SetInLocalStorage(tokenKey, token);
            var authState = BuildAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await _localStorageRepository.RemoveItem(tokenKey);
            httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
        }
    }
}
