using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Grantor
    {
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("apellido")]
        public string Surname { get; set; } = default!;
        [BsonElement("dni")]
        public string Dni { get; set; } = default!;
    }
}
