using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.Account;

namespace SISGED.Client.Shared
{
    public partial class MainLayout
    {
        [Inject]
        private IJSRuntime IJSRuntime { get; set; } = default!;
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private bool drawerOpen = false;
        private readonly string organizationMainPageUrl = "https://www.notarios.org.pe";
        private IJSObjectReference mainLayoutModule = default!;
        private readonly string userName = "anderley";

        private void ToogleDrawer()
        {
            drawerOpen = !drawerOpen;
        }

        protected override async Task OnInitializedAsync()
        {
            mainLayoutModule = await IJSRuntime.InvokeAsync<IJSObjectReference>("import", "../js/main-layout.js");

            await mainLayoutModule.InvokeVoidAsync("hideCircularProgress");

            SessionAccount = await GetSessionAccountAsync();
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
                throw;
            }
        }
    }
}