using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class AssistantDossierStepUpdateDTO
    {
        public AssistantDossierStepUpdateDTO(Assistant assistant, string dossierType, string documentType, DateTime stepEndDate)
        {
            Assistant = assistant;
            DossierType = dossierType;
            StepEndDate = stepEndDate;
            DocumentType = documentType;
        }

        public Assistant Assistant { get; set; } = default!;
        public string DossierType { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public DateTime StepEndDate { get; set; } = default!;
    }
}
