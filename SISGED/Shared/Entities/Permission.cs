using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Permission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("nombre")]
        public String Name { get; set; }
        [BsonElement("tipo")]
        public string Type { get; set; }
        [BsonElement("label")]
        public string Label { get; set; }
        [BsonElement("icono")]
        public string Icon { get; set; }
        [BsonElement("url")]
        public string Url { get; set; }
    }
}
