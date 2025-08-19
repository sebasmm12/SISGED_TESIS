using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Server.Services.Strategies;

public class UserHistoryDocumentStateYearlyStrategy : IUserHistoryDocumentStateStrategy
{
    private readonly IDocumentService _documentService;

    public UserHistoryDocumentStateYearlyStrategy(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public DateFilterDTO DateFilter => DateFilterDTO.Yearly;

    public IEnumerable<string> DateFilters
    {
        get
        {
            var firstYear = DateTime
                .UtcNow
                .AddHours(-5)
                .AddYears(-4)
                .Year;

            return Enumerable
                .Range(firstYear, 5)
                .Select(year => year.ToString());
        }
    }

    public async Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId)
    {
        return await _documentService.GetUserHistoryStateYearlyAsync(userId);
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
                DateFilter = document.CreationDate.Year.ToString(),
            });

        return documentHistoryStates;
    }
}