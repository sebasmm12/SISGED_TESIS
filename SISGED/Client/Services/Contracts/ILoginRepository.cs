using Microsoft.AspNetCore.Components.Authorization;

namespace SISGED.Client.Services.Contracts
{
    public interface ILoginRepository
    {
        Task<AuthenticationState> GetAuthenticationStateAsync();
        Task Login(string token);
        Task Logout();
    }
}
