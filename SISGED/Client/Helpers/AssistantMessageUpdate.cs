using SISGED.Shared.DTOs;

namespace SISGED.Client.Helpers
{
    public class AssistantMessageUpdate
    {
        public AssistantMessageUpdate(
            string dossierType, string documentType, 
            int subStep, RegisteredDocumentDTO? document)
        {
            DossierType = dossierType;
            DocumentType = documentType;
            SubStep = subStep;
            Document = document;
        }

        public AssistantMessageUpdate(string dossierType, string documentType, int subStep) : this(dossierType,
            documentType, subStep, default)
        {
        }

        public string DossierType { get; set; }
        public string DocumentType { get; set; }
        public int SubStep { get; set; }
        public RegisteredDocumentDTO? Document { get; set; }
    }
}
