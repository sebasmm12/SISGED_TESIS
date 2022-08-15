
using SISGED.Shared.Models.Generics.Step;

namespace SISGED.Shared.Models.Requests.Step
{
    public class StepRegisterRequest
    {
        public string DossierName { get; set; } = default!;
        public List<StepDocument> Documents { get; set; } = new();
    }
}
