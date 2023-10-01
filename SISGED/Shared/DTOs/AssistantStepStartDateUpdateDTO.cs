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

        public string Id { get; set; }
        public AssistantStepDTO Assistant { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime LimitDate { get; set; }
    }
}
