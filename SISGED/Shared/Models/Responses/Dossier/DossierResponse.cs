using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Dossier
{
    public class DossierResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Type { get; set; }
        public Entities.Client Client { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Entities.DossierDocument> Documents { get; set; } = new();
        public List<Entities.Document> DocumentsObject { get; set; } = new();
        public List<Entities.Derivation> Derivations { get; set; } = new();
        public string State { get; set; }
    }
}
