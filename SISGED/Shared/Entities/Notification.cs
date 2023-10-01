using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SISGED.Shared.Entities
{
    public class Notification
    {
        public Notification(string senderId, string receiverId, string documentId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            DocumentId = documentId;
        }

        public Notification() { }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("senderId")]
        public string? SenderId { get; set; }
        [BsonElement("receiverId")]
        public string ReceiverId { get; set; } = default!;
        [BsonElement("documentId")]
        public string DocumentId { get; set; } = default!;
        [BsonElement("issueDate")]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("link")]
        public string Link { get; set; } = default!;
        [BsonElement("seen")]
        public bool Seen { get; set; } = false;


    }
}
