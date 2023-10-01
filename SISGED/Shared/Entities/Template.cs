using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SISGED.Shared.Entities
{
    public class Template
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("senderUserType")]
        public string SenderUserType { get; set; } = default!;
        [BsonElement("receiverUserType")]
        public string ReceiverUserType { get; set; } = default!;
        [BsonElement("actionId")]
        public string ActionId { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("link")]
        public string Link { get; set; } = default!;
    }
}
