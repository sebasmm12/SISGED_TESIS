using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Server.Services.Contracts
{
    public interface ITrayService : IGenericService
    {
        Task<InputOutputTrayResponse> GetAsync(string user);
        Task<InputTrayResponse> GetInputStrayAsync(string user);
        Task<DocumentTray> GetDocumentTrayByUserIdDocumentIdAsync(string userId, string documentId);
        Task RegisterUserTrayAsync(string type, string userId);
        Task UpdateTrayForDerivationAsync(UpdateTrayDTO updateTrayDTO);
        Task<string> RegisterUserInputTrayAsync(string dossierId, string documentId, string type);
        Task RegisterOutputTrayAsync(OutPutTrayDTO outPutTrayDTO);
        Task RegisterOutputTrayWithDocumentTrayAsync(DocumentTray document, User user);
        Task<IEnumerable<UserTrayResponse>> GetWorkloadByRoleAsync(string role);
        Task MoveUserOutPutToInputTrayAsync(UserTrayAnnulmentDTO userTrayAnnulmentDTO);
        Task MoveUserTrayAsync(UserTrayAnnulmentDTO userTrayAnnulmentDTO);
    }
}
