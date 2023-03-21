using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class AssistantStepStartDateUpdateDTO
    {
        public AssistantStepStartDateUpdateDTO(Assistant assistant, DateTime startDate, DateTime limitDate)
        {
            Assistant = assistant;
            StartDate = startDate;
            LimitDate = limitDate;
        }

        public Assistant Assistant { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime LimitDate { get; set; }
    }
}
