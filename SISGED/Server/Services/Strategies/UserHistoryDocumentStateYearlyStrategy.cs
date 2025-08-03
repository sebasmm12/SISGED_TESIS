using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Strategies
{
    public class UserHistoryDocumentStateYearlyStrategy : IUserHistoryDocumentStateStrategy
    {
        private readonly IDocumentService _documentService;

        public UserHistoryDocumentStateYearlyStrategy(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        public DateFilterDTO DateFilter => DateFilterDTO.Yearly;

        public async Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId)
        {
            return await _documentService.GetUserHistoryStateYearlyAsync(userId);
        }
    }
