using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Requests.Dossier
{
    public class DossierLastDocumentRequest
    {
        public string Id { get; set; } = default!;
        public List<DossierDocument> Documents { get; set; } = default!;
        public List<Derivation> Derivations { get; set; } = default!;
    }
}
