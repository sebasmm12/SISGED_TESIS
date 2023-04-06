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
        [CascadingParameter(Name = "Notifications")]
        public List<NotificationInfoResponse>? Notifications { get; set; }

        private string GetNotificationState(bool state)
        {
            return NotificationStrategy.GetNotificationStatus(state);
        }

        private void UpdateNotification(NotificationUpdateResponse notificationUpdateResponse)
        {
            var notification = Notifications!.Find(notification => notification.Id == notificationUpdateResponse.Id);

            notification!.Seen = notificationUpdateResponse.Seen;

            Notifications = Notifications
                                .OrderBy(notification => notification.Seen)
                                .ThenByDescending(notification => notification.IssueDate)
                                .ToList();

            StateHasChanged();
        }

    }
}