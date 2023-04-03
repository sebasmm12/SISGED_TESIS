namespace SISGED.Shared.Models.Requests.Notifications
{
    public class NotificationUpdateRequest
    {
        public NotificationUpdateRequest(string notificationId)
        {
            NotificationId = notificationId;
        }

        public string NotificationId { get; set; } = default!;
    }
}
