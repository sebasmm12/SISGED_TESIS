using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SISGED.Client.Shared
{
    public partial class MainLayout
    {
        [Inject]
        private IJSRuntime IJSRuntime { get; set; } = default!;
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository SwalFireRepository { get; set; } = default!;

        public SessionAccountResponse SessionAccount { get; set; } = default!;
        [CascadingParameter]
        public Task<AuthenticationState> authenticationState { get; set; } = default!;

        private bool drawerOpen = false;
        private readonly string organizationMainPageUrl = "https://www.notarios.org.pe";
        private IJSObjectReference mainLayoutModule = default!;
        private string userName = "";

        private void ToogleDrawer()
        {
            drawerOpen = !drawerOpen;
        }

        protected override async Task OnInitializedAsync()
        {
            mainLayoutModule = await IJSRuntime.InvokeAsync<IJSObjectReference>("import", "../js/main-layout.js");

            var authState = await authenticationState;
            
            var user = authState.User;

            if (user.Identity!.IsAuthenticated)
            {
                userName = user.Identity!.Name!;
                SessionAccount = await GetSessionAccountAsync();

            }

            await mainLayoutModule.InvokeVoidAsync("hideCircularProgress");
        }

        private async Task<SessionAccountResponse> GetSessionAccountAsync()
        {
            // TODO: Get the username of the session and put it into the url to get the real items.
            try
            {
                var sessionAccountResponse = await HttpRepository.GetAsync<SessionAccountResponse>($"api/accounts/name/{userName}");

                if (sessionAccountResponse.Error)
                {
                    return new();
                }

                return sessionAccountResponse.Response!;
            }
            catch (Exception)
            {
                // TODO: Implement the logic to send the user a modal saying that we couldn't get the information of their items in the work environment
                await SwalFireRepository.ShowErrorSwalFireAsync("Hubo un error con la sesión de usuario.");
                return new();
            }
        }
    }
}