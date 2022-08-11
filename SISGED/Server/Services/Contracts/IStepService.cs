using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Services.Contracts
{
    public interface IStepService
    {
        Task<List<Steps>> GetStepsAsync();
        Task<Steps> GetStepByDossierNameAsync(String dossierName);
        Task<Steps> GetStepByIdAsync(String stepId);
        Task<List<StepsRequest>> GetStepRequestAsync();
        Task<StepsRequest> RegisterStepAsync(StepsRequest stepsRequest);
        Task<StepsRequest> ModifyStep(StepsRequest stepsRequest)
    }
}
