namespace SISGED.Shared.Models.Requests.Assistants
{
    public class AssistantUpdateRequest
    {
        public AssistantUpdateRequest(string id, string dossierType, string documentType)
        {
            Id = id;
            DossierType = dossierType;
            DocumentType = documentType;
        }

        public string Id { get; set; }
        public string DossierType { get; set; }
        public string DocumentType { get; set; }

    }
}
