using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Requests.Notifications
{
    public class NotificationRegisterRequest
    {
        public NotificationRegisterRequest(string senderUserId, string receiverUserId, NotificationDocument document, string actionId, string type)
        {
            SenderUserId = senderUserId;
            ReceiverUserId = receiverUserId;
            Document = document;
            ActionId = actionId;
            Type = type;
        }

        public string SenderUserId { get; set; } = default!;
        public string ReceiverUserId { get; set; } = default!;
        public NotificationDocument Document { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public string Type { get; set; } = "notificacion";
    }
}
