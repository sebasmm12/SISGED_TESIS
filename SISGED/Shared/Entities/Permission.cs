using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Permission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("label")]
        public string Label { get; set; } = default!;
        [BsonElement("icono")]
        public string Icon { get; set; } = default!;
        [BsonElement("url")]
        public string Url { get; set; } = default!;
    }
}
