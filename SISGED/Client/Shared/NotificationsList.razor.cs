using Microsoft.AspNetCore.Components;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
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


        [CascadingParameter(Name = "SessionAccount")]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private List<NotificationInfoResponse>? notifications;

        protected override async Task OnInitializedAsync()
        {
            notifications = await GetNotificationByUserIdAsync(SessionAccount.User.Id);
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
    }
}