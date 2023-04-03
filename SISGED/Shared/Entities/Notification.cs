using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SISGED.Shared.Entities
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("cuerpo")]
        public string Description { get; set; } = default!;
        [BsonElement("idemisor")]
        public string SenderId { get; set; } = default!;
        [BsonElement("idreceptor")]
        public string ReceiverId { get; set; } = default!;
        [BsonElement("iddocumento")]
        public string DocumentId { get; set; } = default!;
        [BsonElement("fechaemision")]
        public DateTime IssueDate { get; set; } = default!;
        [BsonElement("enlace")]
        public string Link { get; set; } = default!;
        [BsonElement("visto")]
        public bool Seen { get; set; } = false;


    }
}
