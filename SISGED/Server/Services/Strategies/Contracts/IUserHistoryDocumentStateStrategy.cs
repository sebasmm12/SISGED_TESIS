using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Server.Services.Strategies.Contracts;

public interface IUserHistoryDocumentStateStrategy
{
    DateFilterDTO DateFilter { get; }

    IEnumerable<string> DateFilters { get; }

    Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId);

    IEnumerable<UserDocumentHistoryStateDTO> MapToUserDocumentHistoryStates(IEnumerable<DocumentResponse> documents);
}