using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Requests.Assistants
{
    public class AssistantUpdateRequest
    {
        public AssistantUpdateRequest(
            string id, string dossierType, 
            string documentType, RegisteredDocumentDTO? document)
        {
            Id = id;
            DossierType = dossierType;
            DocumentType = documentType;
            Document = document;
        }

        public AssistantUpdateRequest(string id, string dossierType, string documentType) : this(id, dossierType,
            documentType, default)
        {

        }

        public AssistantUpdateRequest()
        {

        }

        public string Id { get; set; }
        
        public string DossierType { get; set; }
        
        public string DocumentType { get; set; }

        public RegisteredDocumentDTO? Document { get; set; }

    }
}
