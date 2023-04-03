namespace SISGED.Shared.Models.Responses.Notification
{
    public class NotificationUpdateResponse
    {
        public NotificationUpdateResponse(string id, bool seen)
        {
            Id = id;
            Seen = seen;
        }

        public string Id { get; set; } = default!;
        public bool Seen { get; set; } = default!;
    }
}
