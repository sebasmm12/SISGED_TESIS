namespace SISGED.Shared.Models.Requests.Step
{
    public class StepUpdateRequest: StepRegisterRequest
    {
        public string Id { get; set; } = default!;
    }
}
