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

        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public NotificationDocument Document { get; set; }
        public string ActionId { get; set; }
        public string Type { get; set; } = "notificacion";
    }
}
