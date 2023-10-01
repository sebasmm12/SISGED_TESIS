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
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("lastName")]
        public string LastName { get; set; } = default!;
        [BsonElement("documentNumber")]
        public string DocumentNumber { get; set; } = default!;
        [BsonElement("documentType")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("clientId")]
        public string ClientId { get; set; } = default!;

        public string GetShortName()
        {
            return GetFirstName(Name) + " " + GetFirstName(LastName);
        }

        public string GetAvatarName()
        {
            return GetFirstLetter(Name) + GetFirstLetter(LastName);
        }
        
        private static string GetFirstLetter(string name)
        {
            return name.Trim().First().ToString();
        }

        private static string GetFirstName(string name)
        {
            return name.Trim().Split(" ")[0];
        }
    }
}
