using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies
{
    public class UserHistoryDocumentStateMonthlyStrategy : IUserHistoryDocumentStateStrategy
    {
        private readonly IDocumentService _documentService;

        public UserHistoryDocumentStateMonthlyStrategy(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        public DateFilterDTO DateFilter => DateFilterDTO.Monthly;

        public async Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId)
        {
            return await _documentService.GetUserHistoryStateMonthlyAsync(userId);
        }
    }
