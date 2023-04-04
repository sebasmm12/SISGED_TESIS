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
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("cuerpo")]
        public string Description { get; set; } = default!;
        [BsonElement("idemisor")]
        public string? SenderId { get; set; }
        [BsonElement("idreceptor")]
        public string ReceiverId { get; set; } = default!;
        [BsonElement("iddocumento")]
        public string DocumentId { get; set; } = default!;
        [BsonElement("fechaemision")]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("enlace")]
        public string Link { get; set; } = default!;
        [BsonElement("visto")]
        public bool Seen { get; set; } = false;


    }
}
