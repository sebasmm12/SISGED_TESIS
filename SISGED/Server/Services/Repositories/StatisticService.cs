using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Repositories
{
    public class StatisticService : IStatisticService
    {
        private readonly IDocumentService _documentService;
        private readonly IDossierService _reportService;

        public StatisticService(IDocumentService documentService, IDossierService reportService)
        {
            _documentService = documentService;
            _reportService = reportService;
        }

        public async Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAndAreaAsync(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery)
        {
            return await _documentService.GetDocumentsByMonthAndAreaAsync(documentsByMonthAndAreaQuery);
        }

        public async Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery)
        {
            return await _documentService.GetDocumentsByMonthAsync(documentsByMonthQuery);
        }

        public async Task<IEnumerable<ExpiredDocumentsResponse>> GetExpiredDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery)
        {
            return await _documentService.GetExpiredDocumentsByMonthAsync(documentsByMonthQuery);
        }
        
        public async Task<IEnumerable<DocumentByStateResponse>> GetDocumentsByStateAsync(DocumentsByStateQuery documentsByStateQuery)
        {
            return await _documentService.GetDocumentsByStateAsync(documentsByStateQuery);
        }

        public async Task<IEnumerable<DossierGanttDiagramResponse>> GetDossierGanttDiagramAsync(DossierGanttDiagramQuery dossierGanttDiagramQuery)
        {
            return await _reportService.GetDossierGanttDiagramAsync(dossierGanttDiagramQuery);
        }

    }
}
