using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace SISGED.Client.Services.Repositories
{
    public class LoginRepository : AuthenticationStateProvider, ILoginRepository
    {
        private HttpClient httpClient { get; set; } = default!;
        private readonly ILocalStorageRepository _localStorageRepository;
        private readonly IHttpRepository _httpRepository;

        public LoginRepository(ILocalStorageRepository localStorageRepository, HttpClient http, IHttpRepository httpRepository)
        {
            _localStorageRepository = localStorageRepository;
            httpClient = http;
            _httpRepository = httpRepository;
        }

        private readonly string tokenKey = "TOKENKEY";
        private readonly string expirationTokenKey = "EXPIRATIONTOKENKEY";

        private AuthenticationState anonymous => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageRepository.GetFromLocalStorage(tokenKey)!;

            if (token is null)
            {
                return anonymous;
            }

            var expirationTimeObject = await _localStorageRepository.GetFromLocalStorage(expirationTokenKey)!;
            if (expirationTimeObject is null)
            {
                await CleanLocalStorage();
                return anonymous;
            }

            if (DateTime.TryParse(expirationTimeObject.ToString(), out var expirationTime))
            {
                if (IsExpired(expirationTime))
                {
                    await CleanLocalStorage();
                    return anonymous;
                }
                else if (MustBeRevenewed(expirationTime))
                {
                    token = await RenewToken(token);
                    if (token is null)
                    {
                        await CleanLocalStorage();
                        return anonymous;
                    }
                }
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
        public async Task Login(UserToken token)
        {
            await _localStorageRepository.SetInLocalStorage(tokenKey, token.Token);
            await _localStorageRepository.SetInLocalStorage(expirationTokenKey, token.Expiration.ToString());
            var authState = BuildAuthenticationState(token.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await CleanLocalStorage();
            NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
        }

        private async Task CleanLocalStorage()
        {
            await _localStorageRepository.RemoveItem(tokenKey);
            await _localStorageRepository.RemoveItem(expirationTokenKey);
            httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private bool IsExpired(DateTime time)
        {
            return time <= DateTime.UtcNow;
        }

        private bool MustBeRevenewed(DateTime time) 
        {
            return time.Subtract(DateTime.UtcNow) <= TimeSpan.FromHours(1);
        }

        private async Task<string?> RenewToken(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            var httpResponse = await _httpRepository.GetAsync<UserToken>("api/accounts/renewToken");

            if (httpResponse.Error)
            {
                return null;
            }

            var newToken = httpResponse.Response!;
            await _localStorageRepository.SetInLocalStorage(tokenKey, newToken.Token);
            await _localStorageRepository.SetInLocalStorage(expirationTokenKey, newToken.Expiration.ToString());

            return newToken.Token;
        }

        public async Task ManageTokenRenew()
        {
            var expirationTimeObject = await _localStorageRepository.GetFromLocalStorage(expirationTokenKey)!;
            if(DateTime.TryParse(expirationTimeObject.ToString(), out var expirationTime))
            {
                if(IsExpired(expirationTime))
                {
                    await Logout();
                }

                if (MustBeRevenewed(expirationTime))
                {
                    var token = await _localStorageRepository.GetFromLocalStorage(tokenKey)!;
                    var newToken = await RenewToken(token);
                    if (newToken is not null)
                    {
                        var authState = BuildAuthenticationState(newToken);
                        NotifyAuthenticationStateChanged(Task.FromResult(authState));
                    }
                }
            }
        }
    }
}
