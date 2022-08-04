using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("label")]
        public string Label { get; set; } = default!;
        [BsonElement("listaherramientas")]
        public List<string> Tools { get; set; } = new List<string>();
        [BsonElement("listainterfaces")]
        public List<String> Interfaces { get; set; } = new List<string>();
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
    }
}
