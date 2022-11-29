using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Client
    {
        public Client() { }

        public Client(string name, string documentNumber, string documentType)
        {
            Name = name;
            DocumentNumber = documentNumber;
            DocumentType = documentType;
        }

        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("numerodocumento")]
        public string DocumentNumber { get; set; } = default!;
        [BsonElement("tipodocumento")]
        public string DocumentType { get; set; } = default!;
    }
}
