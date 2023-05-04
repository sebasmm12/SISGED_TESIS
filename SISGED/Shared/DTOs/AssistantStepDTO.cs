namespace SISGED.Shared.DTOs
{
    public class AssistantStepDTO
    {
        public AssistantStepDTO(int step, string documentType, string dossierType)
        {
            Step = step;
            DocumentType = documentType;
            DossierType = dossierType;
        }

        public int Step { get; set; }
        public string DocumentType { get; set; } = default!;
        public string DossierType { get; set; } = default!;
    }
}
