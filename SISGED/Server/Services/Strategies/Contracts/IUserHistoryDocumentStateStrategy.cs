using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies.Contracts
{
    public interface IUserHistoryDocumentStateStrategy
    {
        DateFilterDTO DateFilter { get; }

        Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId);
    }
}
