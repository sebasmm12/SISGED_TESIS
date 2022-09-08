using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Client
    {
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("numerodocumento")]
        public string DocumentNumber { get; set; } = default!;
        [BsonElement("tipodocumento")]
        public string DocumentType { get; set; } = default!;
    }
}
