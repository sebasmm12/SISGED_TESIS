namespace SISGED.Shared.Models.Requests.Assistants
{
    public class AssistantCreateRequest
    {
        public AssistantCreateRequest(string dossierId, string dossierName)
        {
            DossierId = dossierId;
            DossierName = dossierName;
        }

        public string DossierId { get; set; }
        public string DossierName { get; set; }
    }
}
