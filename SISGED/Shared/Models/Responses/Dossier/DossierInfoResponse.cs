using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Dossier
{
    public class DossierInfoResponse
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Entities.DossierDocument> Documents { get; set; } = default!;
        public List<Derivation> Derivations { get; set; } = default!;
        public string State { get; set; } = default!;
    }
}
