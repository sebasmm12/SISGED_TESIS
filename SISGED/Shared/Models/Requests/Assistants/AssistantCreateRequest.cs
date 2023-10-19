using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Requests.Assistants
{
    public class AssistantCreateRequest
    {
        public AssistantCreateRequest(string dossierId, string dossierName, RegisteredDocumentDTO document)
        {
            DossierId = dossierId;
            DossierName = dossierName;
            Document = document;
        }

        public string DossierId { get; set; }

        public string DossierName { get; set; }

        public RegisteredDocumentDTO Document { get; set; }
    }
}
