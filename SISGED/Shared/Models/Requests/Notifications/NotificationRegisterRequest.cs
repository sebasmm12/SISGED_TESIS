using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Requests.Notifications
{
    public class NotificationRegisterRequest
    {
        public string SenderUserId { get; set; } = default!;
        public string ReceiverUserId { get; set; } = default!;
        public NotificationDocument Document { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public string Type { get; set; } = "notificacion";
    }
}
