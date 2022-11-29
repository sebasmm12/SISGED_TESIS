using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class DocumentType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
    }
}
