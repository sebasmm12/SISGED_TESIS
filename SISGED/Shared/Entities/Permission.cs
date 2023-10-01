using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Permission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("label")]
        public string Label { get; set; } = default!;
        [BsonElement("icon")]
        public string Icon { get; set; } = default!;
        [BsonElement("url")]
        public string Url { get; set; } = default!;
    }
}
