using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Contracts
{
    public interface IDossierService : IGenericService
    {
        Task CreateDossierAsync(Dossier dossier);
        Task<IEnumerable<DossierGanttDiagramResponse>> GetDossierGanttDiagramAsync(DossierGanttDiagramQuery dossierGanttDiagramQuery);
        Task<IEnumerable<Dossier>> GetDossiersAsync();
        Task<DossierLastDocumentResponse> RegisterDerivationAsync(DossierLastDocumentRequest dossierLastDocumentRequest, string userId);
    }
}
