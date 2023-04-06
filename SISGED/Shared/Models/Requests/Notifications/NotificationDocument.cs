namespace SISGED.Shared.Models.Requests.Notifications
{
    public class NotificationDocument
    {
        public NotificationDocument(string documentId, string title)
        {
            DocumentId = documentId;
            Title = title;
        }

        public string DocumentId { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}
