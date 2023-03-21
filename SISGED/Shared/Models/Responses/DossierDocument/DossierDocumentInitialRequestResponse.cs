using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.DossierDocument
{
    public class DossierDocumentInitialRequestResponse
    {
        public Entities.Dossier Dossier { get; set; } = default!;
        public InitialRequest InitialRequest { get; set; } = default!;

        public DossierDocumentInitialRequestResponse() { }

        public DossierDocumentInitialRequestResponse(Entities.Dossier dossier, InitialRequest initialRequest)
        {
            Dossier = dossier;
            InitialRequest = initialRequest;
        }
    }
}
