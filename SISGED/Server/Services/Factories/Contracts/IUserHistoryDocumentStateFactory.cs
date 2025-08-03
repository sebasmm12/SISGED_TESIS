using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Factories.Contracts
{
    public interface IUserHistoryDocumentStateFactory
    {
        Task<IEnumerable<UserDocumentHistoryStateResponse>> GetUserHistoryDocumentStateAsync(string userId, DateFilterDTO dateFilter);
    }
}
