using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Shared.Models.Responses.DossierTray
{
    public class DossierTrayResponse
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("dossierId")]
        public string DossierId { get; set; } = default!;
        [BsonElement("client")]
        public Client? Client { get; set; }
        [BsonElement("document")]
        public DocumentResponse? Document { get; set; }
        [BsonElement("documentObjects")]
        public List<DocumentResponse>? DocumentObjects { get; set; }
        [BsonElement("type")]
        public string? Type { get; set; }
    }
}
