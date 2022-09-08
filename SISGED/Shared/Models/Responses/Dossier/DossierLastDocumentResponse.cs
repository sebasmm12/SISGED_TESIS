using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Generics.Document;

namespace SISGED.Shared.Models.Responses.Dossier
{
    public class DossierLastDocumentResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("client")]
        public Client Client { get; set; } = new();
        [BsonElement("lastDocument")]
        public DocumentInfo LastDocument { get; set; } = new();
        [BsonElement("documents")]
        public List<DocumentInfo> Documents { get; set; } = new();
        [BsonElement("type")]
        public string Type { get; set; } = default!;
    }
}
