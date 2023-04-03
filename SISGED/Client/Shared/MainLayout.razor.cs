using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;

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
        [Inject]
        private ITokenRenewer TokenRenewer { get; set; } = default!;

        public SessionAccountResponse SessionAccount { get; set; } = default!;
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = default!;

        private bool drawerOpen = false;
        private readonly string organizationMainPageUrl = "https://www.notarios.org.pe";
        private IJSObjectReference mainLayoutModule = default!;
        private string userName = "";
        private bool _render;

        private void ToogleDrawer()
        {
            drawerOpen = !drawerOpen;
        }

        protected override async Task OnInitializedAsync()
        {
            TokenRenewer.Start();

            mainLayoutModule = await IJSRuntime.InvokeAsync<IJSObjectReference>("import", "../js/main-layout.js");

            var authState = await AuthenticationState;

            var user = authState.User;

            if (user.Identity!.IsAuthenticated)
            {
                userName = user.Identity!.Name!;
                SessionAccount = await GetSessionAccountAsync();

            }

            await mainLayoutModule.InvokeVoidAsync("hideCircularProgress");
            _render = true;
        }

        private async Task<SessionAccountResponse> GetSessionAccountAsync()
        {
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
                await SwalFireRepository.ShowErrorSwalFireAsync("Hubo un error con la sesión de usuario.");
                return new();
            }
        }
    }
}