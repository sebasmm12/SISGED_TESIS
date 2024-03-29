﻿namespace SISGED.Shared.DTOs
{
    public class AssistantStepUpdateDTO
    {
        public string Id { get; set; } = default!;
        public AssistantStepDTO LastAssistantStep { get; set; } = default!;
        public AssistantStepDTO NewAssistantStep { get; set; } = default!;
        public string DossierType { get; set; } = default!;
        public DateTime EndDate { get; set; }

        public AssistantStepUpdateDTO()
        {

        }

        public AssistantStepUpdateDTO(DateTime endDate, AssistantStepDTO lastAssistantStep)
        {
            EndDate = endDate;
            LastAssistantStep = lastAssistantStep;
        }
    }
}
