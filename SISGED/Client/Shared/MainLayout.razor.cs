using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Notification;

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
        [Inject]
        private IWebAssemblyHostEnvironment WebAssemblyHostEnvironment { get; set; } = default!;

        public SessionAccountResponse SessionAccount { get; set; } = default!;
        public List<NotificationInfoResponse>? Notifications { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = default!;

        private bool drawerOpen = false;
        private readonly string organizationMainPageUrl = "https://www.notarios.org.pe";
        private IJSObjectReference mainLayoutModule = default!;
        private string userName = "";
        private bool _render;
        private HubConnection? _notificationHubConnection = default!;
        private readonly string notificationMethodName = "RecieveNotificationAsync";

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

                await GetNotificationsByUserAsync();
            }

            await mainLayoutModule.InvokeVoidAsync("hideCircularProgress");

            _render = true;
        }

        private async Task GetNotificationsByUserAsync()
        {
            Notifications = await GetNotificationByUserIdAsync(SessionAccount.User.Id);

            await StartHubConnectionAsync();

            SetRefreshNotificationListener();
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

        private async Task<List<NotificationInfoResponse>> GetNotificationByUserIdAsync(string userId)
        {
            try
            {
                var notificationResponse = await HttpRepository.GetAsync<List<NotificationInfoResponse>>($"api/notifications/{userId}");

                if (notificationResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener sus última notificaciones");
                }

                return notificationResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener sus última notificaciones");

                return new List<NotificationInfoResponse>();
            }
        }

        private async Task StartHubConnectionAsync()
        {
            var apiAddress = WebAssemblyHostEnvironment.BaseAddress;

            var sensorDataUrl = ($"{apiAddress}notifications?userId={SessionAccount.User.Id}");
            _notificationHubConnection = new HubConnectionBuilder()
                            .WithUrl(sensorDataUrl)
                            .Build();


            await _notificationHubConnection.StartAsync();
        }

        private void SetRefreshNotificationListener()
        {
            _notificationHubConnection!.On<NotificationInfoResponse>(notificationMethodName, notificationInfoResponse =>
            {
                if (Notifications is null) return;

                if (Notifications.Count > 10) Notifications.RemoveAt(Notifications.Count - 1);

                Notifications.Insert(0, notificationInfoResponse);
            });
        }
    }
}