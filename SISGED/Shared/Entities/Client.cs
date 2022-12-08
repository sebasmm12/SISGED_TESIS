using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Client
    {
        public Client() { }

        public Client(string name, string lastName, string documentNumber, string documentType, string id)
        {
            ClientId = id;
            Name = name;
            LastName = lastName;
            DocumentNumber = documentNumber;
            DocumentType = documentType;
        }
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("apellido")]
        public string LastName { get; set; } = default!;
        [BsonElement("numerodocumento")]
        public string DocumentNumber { get; set; } = default!;
        [BsonElement("tipodocumento")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("idcliente")]
        public string ClientId { get; set; } = default!;

        public string GetShortName()
        {
            return GetFirstName(Name) + " " + GetFirstName(LastName);
        }

        private static string GetFirstName(string name)
        {
            return name.Trim().Split(" ")[0];
        }
    }
}
