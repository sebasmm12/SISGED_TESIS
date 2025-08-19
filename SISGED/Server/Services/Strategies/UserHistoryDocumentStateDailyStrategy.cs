using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Server.Services.Strategies;

public class UserHistoryDocumentStateDailyStrategy : IUserHistoryDocumentStateStrategy
{
    private readonly IDocumentService _documentService;

    public UserHistoryDocumentStateDailyStrategy(IDocumentService documentService)
    {
        _documentService = documentService;
    }
    public DateFilterDTO DateFilter => DateFilterDTO.Daily;

    public IEnumerable<string> DateFilters
    {
        get
        {
            var lastDate = DateTime
                .UtcNow
                .AddHours(-5)
                .AddDays(-6);

            return Enumerable
                .Range(0, 7)
                .Select(i => lastDate.AddDays(i).ToString("dd/MM/yyyy"));
        }
    }

    public async Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId)
    {
        return await _documentService.GetUserHistoryStateDailyAsync(userId);
    }

    public IEnumerable<UserDocumentHistoryStateDTO> MapToUserDocumentHistoryStates(IEnumerable<DocumentResponse> documents)
    {
        var documentHistoryStates = documents
            .Select(document => new UserDocumentHistoryStateDTO
            {
                Id = document.Id,
                State = document.State,
                EndDate = document.EndDate,
                DueDate = document.DueDate,
                DateFilter = $"{document.CreationDate.Day}/{document.CreationDate.Month}/{document.CreationDate.Year}"
            });

        return documentHistoryStates;
    }
}
