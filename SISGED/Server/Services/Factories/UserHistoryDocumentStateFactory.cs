using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Factories.Contracts;
using SISGED.Server.Services.Strategies.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Server.Services.Factories
{
    public class UserHistoryDocumentStateFactory : IUserHistoryDocumentStateFactory
    {
        private readonly IEnumerable<IUserHistoryDocumentStateStrategy> _userHistoryDocumentStateStrategies;
        private readonly ITrayService trayService;
        private readonly IDocumentService documentService;

        public UserHistoryDocumentStateFactory(
            IEnumerable<IUserHistoryDocumentStateStrategy> userHistoryDocumentStateStrategies, 
            ITrayService trayService, 
            IDocumentService documentService)
        {
            _userHistoryDocumentStateStrategies = userHistoryDocumentStateStrategies;
            this.trayService = trayService;
            this.documentService = documentService;
        }

        public async Task<IEnumerable<UserDocumentHistoryStateResponse>> GetUserHistoryDocumentStateAsync(string userId, DateFilterDTO dateFilter)
        {
            var userHistoryDocumentStateStrategy =  _userHistoryDocumentStateStrategies.FirstOrDefault(x => x.DateFilter == dateFilter);

            if (userHistoryDocumentStateStrategy is null) 
                throw new ArgumentException("Filtro de fechas inválido para la obtención de los documentos del usuaio por estado");

            var documentsTask = userHistoryDocumentStateStrategy.GetDocumentsAsync(userId);

            var userTrayDocumentsTask = GetUserTrayDocuments(userId);

            await Task.WhenAll(documentsTask, userTrayDocumentsTask);

            var documents = await documentsTask;

            var userTrayDocuments = userHistoryDocumentStateStrategy.MapToUserDocumentHistoryStates(await userTrayDocumentsTask);

            var groupedDocuments = userHistoryDocumentStateStrategy
                .DateFilters
                .Select(date => GetDocumentsByDate(date, documents, userTrayDocuments));

            return groupedDocuments;
        }

        private UserDocumentHistoryStateResponse GetDocumentsByDate(
            string dateFilter, IEnumerable<UserDocumentHistoryStateDTO> documents, IEnumerable<UserDocumentHistoryStateDTO> userTrayDocuments)
        {
            var filteredDocuments = documents
                .Where(document => document.DateFilter == dateFilter)
                .ToList();

            var dueDocuments = userTrayDocuments
                .Where(document => document.DateFilter == dateFilter && IsDocumentExpired(document))
                .ToList();

            var approvedDocuments = filteredDocuments
                .Where(document => document.IsApproved.HasValue && document.IsApproved.Value)
                .ToList();

            var rejectedDocuments = filteredDocuments
                .Where(document => document.IsApproved.HasValue && !document.IsApproved.Value)
                .ToList();

            var nonDueDocuments = filteredDocuments
                .Except(approvedDocuments)
                .Except(rejectedDocuments);

            var groupedDocuments = nonDueDocuments
                .GroupBy(document => document.State)
                .ToDictionary(groupedDocument => groupedDocument.Key, groupedDocument => groupedDocument.Count());
           
            SetAdditionalDocumentStates(
                groupedDocuments,
                dueDocuments.Count,
                approvedDocuments.Count,
                rejectedDocuments.Count);

            return new(dateFilter, groupedDocuments);
        }

        private static bool IsDocumentExpired(UserDocumentHistoryStateDTO document)
        {
            var currentDate = DateTime.UtcNow.AddHours(-5);

            return !document.EndDate.HasValue && document.DueDate.Date < currentDate.Date;
        }

        private static void SetAdditionalDocumentStates(
            Dictionary<string, int> documentsByState,
            int dueDocuments,
            int approvedDocuments,
            int rejectedDocuments)
        {
            documentsByState.Add("retrasado", dueDocuments);
            documentsByState.Add("aprobado", approvedDocuments);
            documentsByState.Add("rechazado", rejectedDocuments);
        }

        private async Task<IEnumerable<DocumentResponse>> GetUserTrayDocuments(string userId)
        {
            var userTrays = await trayService.GetTrayDocumentAsync(userId);

            var userTrayDocumentIds = userTrays
                .InputTray
                .Concat(userTrays.OutputTray)
                .Select(tray => tray.DocumentId)
                .ToList();

            var documents = await documentService.GetDocumentsAsync(userTrayDocumentIds);

            return documents;
        }
    }
}
