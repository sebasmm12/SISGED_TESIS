using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;
using SISGED.Shared.Models.Responses.Step;

namespace SISGED.Server.Services.Contracts
{
    public interface IStepService
    {
        Task<List<Step>> GetStepsAsync();
        Task<Step> GetStepByDossierTypeAsync(string dossierName);
        Task<Step> GetStepByIdAsync(string stepId);
        Task<IEnumerable<DossierStepsResponse>> GetStepRequestAsync();
        Task RegisterStepAsync(StepRegisterRequest stepsRequest);
        Task UpdateStepAsync(StepUpdateRequest stepsRequest);
    }
}
