using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.DossierDocument
{
    public class DossierDocumentInitialRequestResponse
    {
        public Entities.Dossier Dossier { get; set; } = default!;
        
        public InitialRequest InitialRequest { get; set; } = default!;
    
        public string ReceiverUserId { get; set; } = string.Empty;

        public DossierDocumentInitialRequestResponse() { }

        public DossierDocumentInitialRequestResponse(
            Entities.Dossier dossier, 
            InitialRequest initialRequest, 
            string receiverUserId)
        {
            Dossier = dossier;
            InitialRequest = initialRequest;
            ReceiverUserId = receiverUserId;
        }
    }
}
