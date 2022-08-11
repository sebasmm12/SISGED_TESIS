namespace SISGED.Shared.Models.Requests.Assistant
{
    public class AssistantCreateRequest
    {
        public string DossierId { get; set; } = default!;
        public string DossierName { get; set; } = default!;
    }
}
