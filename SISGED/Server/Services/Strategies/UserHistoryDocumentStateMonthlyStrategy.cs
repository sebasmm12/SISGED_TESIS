using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Document;

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

        public IEnumerable<string> DateFilters
        {
            get
            {
                var lastDate = DateTime
                    .UtcNow
                    .AddHours(-5)
                    .AddMonths(-11);

                return Enumerable
                    .Range(0, 12)
                    .Select(i =>
                    {
                        var date = lastDate.AddMonths(i);

                        return $"{MonthNames[date.Month]},{date.Year}";
                    });
            }
        }

        private readonly Dictionary<int, string> MonthNames = new()
        {
            { 1, "Enero" },
            { 2, "Febrero" },
            { 3, "Marzo" },
            { 4, "Abril" },
            { 5, "Mayo" },
            { 6, "Junio" },
            { 7, "Julio" },
            { 8, "Agosto" },
            { 9, "Septiembre" },
            { 10, "Octubre" },
            { 11, "Noviembre" },
            { 12, "Diciembre" }
        };

        public async Task<IEnumerable<UserDocumentHistoryStateDTO>> GetDocumentsAsync(string userId)
        {
            var result = await _documentService.GetUserHistoryStateMonthlyAsync(userId);

            ReplaceMonths(result);

            return result;
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
                    DateFilter = $"{MonthNames[document.CreationDate.Month]},{document.CreationDate.Year}"
                });

            return documentHistoryStates;
        }

        private void ReplaceMonths(IEnumerable<UserDocumentHistoryStateDTO> documents)
        {
            foreach (var document in documents)
            {
                int.TryParse(document.DateFilter.Split(',').First(), out var monthFilter);

                var yearFilter = document.DateFilter.Split(',').Last();

                document.DateFilter = $"{MonthNames[monthFilter]},{yearFilter}";
            }
        }
    }
}
