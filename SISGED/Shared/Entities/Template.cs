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
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("tipousuarioemisor")]
        public string SenderUserType { get; set; } = default!;
        [BsonElement("tipousuarioreceptor")]
        public string ReceiverUserType { get; set; } = default!;
        [BsonElement("idaccion")]
        public string ActionId { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("enlace")]
        public string Link { get; set; } = default!;
    }
}
