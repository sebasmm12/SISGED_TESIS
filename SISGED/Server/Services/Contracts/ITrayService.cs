using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Server.Services.Contracts
{
    public interface ITrayService : IGenericService
    {
        Task RegisterUserTrayAsync(string type, string userId);
        Task<InputOutputTrayResponse> GetAsync(string user);
        Task<InputTrayResponse> GetInputStrayAsync(string user);
        Task<Tray> GetTrayDocumentAsync(string user);
    }
}
