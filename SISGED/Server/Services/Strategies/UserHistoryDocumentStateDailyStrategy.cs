using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies;

public class UserHistoryDocumentStateDailyStrategy : IUserHistoryDocumentStateStrategy
{
    private readonly IDocumentService _documentService;

    public UserHistoryDocumentStateDailyStrategy(IDocumentService documentService)
    {
        _documentService = documentService;
    }
    public DateFilterDTO DateFilter => DateFilterDTO.Daily;

    public async Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId)
    {
        return await _documentService.GetUserHistoryStateDailyAsync(userId);
    }
}
