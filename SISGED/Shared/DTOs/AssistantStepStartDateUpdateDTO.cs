using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class AssistantStepStartDateUpdateDTO
    {
        public AssistantStepStartDateUpdateDTO(string id, AssistantStepDTO assistant, DateTime startDate, DateTime limitDate)
        {
            Id = id;
            Assistant = assistant;
            StartDate = startDate;
            LimitDate = limitDate;
        }

        public string Id { get; set; } = default!;
        public AssistantStepDTO Assistant { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime LimitDate { get; set; }
    }
}
