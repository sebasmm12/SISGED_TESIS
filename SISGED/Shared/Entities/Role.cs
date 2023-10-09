using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("label")]
        public string Label { get; set; } = default!;
        [BsonElement("tools")]
        public List<string> Tools { get; set; } = new();
        [BsonElement("interfaces")]
        public List<string> Interfaces { get; set; } = new();
        [BsonElement("description")]
        public string Description { get; set; } = default!;

        public Role()
        {
            
        }

        public Role(string id)
        {
            Id = id;
        }
    }
}
