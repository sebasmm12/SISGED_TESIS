using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Assistants;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Services.Contracts
{
    public interface IAssistantService
    {
        Task<Assistant> GetAssistantByDossierAsync(string dossierId);
        Task<Assistant> CreateAsync(Assistant assistant);
        Task<Assistant> UpdateInitialRequestAsync(Assistant assistant, string dossierName);
        Task<Assistant> UpdateAsync(StepsUpdateRequest stepUpdateRequest);
        Task<Assistant> UpdateFinallyAsync(StepsUpdateRequest stepUpdateRequest);
        Task<Assistant> UpdateNormalAsync(StepsUpdateRequest stepUpdateRequest);
        Task<Assistant> GetAssistantAsync(string assistantId);
        Task<Assistant> UpdateAssistantStepStartDateAsync(AssistantStepStartDateUpdateDTO stepStartDateUpdateRequest);
        Task<Assistant> UpdateAssistantStepAsync(AssistantStepUpdateDTO assistantStepUpdate);
        Task<Assistant> UpdateAssistantDossierAsync(Assistant assistant, AssistantDossierUpdateDTO assistantDossierUpdate);
        Task<Assistant> UpdateAssistantDocumentLastStepAsync(string assistantId, AssistantStepDTO assistantStepDTO);
    }
}
