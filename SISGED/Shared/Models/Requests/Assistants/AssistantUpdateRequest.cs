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

        public string Id { get; set; } = default!;
        public string DossierType { get; set; } = default!;
        public string DocumentType { get; set; } = default!;

    }
}
