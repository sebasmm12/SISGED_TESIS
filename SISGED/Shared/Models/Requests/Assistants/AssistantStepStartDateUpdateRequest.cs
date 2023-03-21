namespace SISGED.Shared.Models.Requests.Assistants
{
    public class AssistantStepStartDateUpdateRequest
    {
        public AssistantStepStartDateUpdateRequest(string assistantId)
        {
            AssistantId = assistantId;
        }

        public string AssistantId { get; set; } = default!;
    }
}
