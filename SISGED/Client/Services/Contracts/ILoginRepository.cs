using Microsoft.AspNetCore.Components.Authorization;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Services.Contracts
{
    public interface ILoginRepository
    {
        Task<AuthenticationState> GetAuthenticationStateAsync();
        Task Login(UserToken token);
        Task Logout();
        Task ManageTokenRenew();
    }
}
