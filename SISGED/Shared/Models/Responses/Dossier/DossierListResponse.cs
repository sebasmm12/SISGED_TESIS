using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.UserDocument;
using SISGED.Shared.Models.Responses.Derivation;

namespace SISGED.Shared.Models.Responses.Dossier
{
    public class DossierListResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("client")]
        public Client Client { get; set; } = default!;
        [BsonElement("startDate")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }
        [BsonElement("documents")]
        public List<UserDocumentResponse> Documents { get; set; } = new();
        [BsonElement("derivations")]
        public List<DossierListDerivationResponse> Derivations { get; set; } = new();
        [BsonElement("state")]
        public string State { get; set; } = default!;
    }
}
