using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Client.Shared
{
    public partial class NotificationsList
    {
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        private NotificationStrategy NotificationStrategy { get; set; } = default!;
        [Inject]
        private ILocalStorageRepository LocalStorageRepository { get; set; } = default!;

        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private List<NotificationInfoResponse>? notifications;

        private HubConnection? _notificationHubConnection = default!;

        private readonly string tokenKey = "TOKENKEY";

        protected override async Task OnInitializedAsync()
        {
            notifications = await GetNotificationByUserIdAsync(SessionAccount.User.Id);
            await StartHubConnection();
            SetRefreshNotificationListener();
        }

        private string GetNotificationState(bool state)
        {
            return NotificationStrategy.GetNotificationStatus(state);
        }

        private void UpdateNotification(NotificationUpdateResponse notificationUpdateResponse)
        {
            var notification = notifications!.Find(notification => notification.Id == notificationUpdateResponse.Id);

            notification!.Seen = notificationUpdateResponse.Seen;

            notifications = notifications
                                .OrderBy(notification => notification.Seen)
                                .ThenByDescending(notification => notification.IssueDate)
                                .ToList();

            StateHasChanged();
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

        private async Task StartHubConnection()
        {
            var apiAddress = "https://localhost:7007";

            //var tokenString = await LocalStorageRepository.GetFromLocalStorage(tokenKey);
            var sensorDataUrl = ($"{apiAddress}/notificationHub");
            _notificationHubConnection = new HubConnectionBuilder()
                            .WithUrl(sensorDataUrl)
                            //.WithUrl(sensorDataUrl, options =>
                            //{
                            //    options.AccessTokenProvider = () => Task.FromResult(tokenString!);
                            //})
                            //.WithAutomaticReconnect()
                            .Build();


            await _notificationHubConnection.StartAsync();
        }

        private void SetRefreshNotificationListener()
        {
            var methodName = "RecieveNotification";


            _notificationHubConnection!.On<string>(methodName, async (userId) =>
            {
                if (userId.Equals(SessionAccount.User.Id))
                {
                    notifications = await GetNotificationByUserIdAsync(SessionAccount.User.Id);
                    StateHasChanged();
                }
            });
        }
    }
}