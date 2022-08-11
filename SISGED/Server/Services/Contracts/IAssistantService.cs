using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Services.Contracts
{
    public interface IAssistantService
    {
        Task<Assistant> GetAssistantAsync(string dossierId);
        Task<Assistant> CreateAsync(Assistant assistant);
        Task<Assistant> UpdateInitialRequestAsync(Assistant assistant, string dossierName);
        Task<Assistant> UpdateAsync(StepsUpdateRequest stepUpdateRequest);
        Task<Assistant> UpdateFinallyAsync(StepsUpdateRequest stepUpdateRequest);
        Task<Assistant> UpdateNormalAsync(StepsUpdateRequest stepUpdateRequest);
    }
}
