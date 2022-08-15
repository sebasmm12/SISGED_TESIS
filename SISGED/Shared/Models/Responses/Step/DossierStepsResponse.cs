using SISGED.Shared.Models.Generics.Step;

namespace SISGED.Shared.Models.Responses.Step
{
    public class DossierStepsResponse
    {
        public string Id { get; set; } = default!;
        public string DossierName { get; set; } = default!;
        public List<StepDocument> Documents { get; set; } = new();
    }
}
