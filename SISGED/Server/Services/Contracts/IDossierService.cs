using MongoDB.Bson;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Dossier;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Contracts
{
    public interface IDossierService : IGenericService
    {
        Task CreateDossierAsync(Dossier dossier);
        Task<IEnumerable<T>> ExecuteDossierAggregateAsync<T>(BsonDocument[] pipelines);
        Task<Dossier> GetDossierAsync(string dossierId);
        Task<IEnumerable<Dossier>> GetDossierByFiltersAsync(DossierHistoryQuery dossierHistoryQuery);
        Task<IEnumerable<DossierGanttDiagramResponse>> GetDossierGanttDiagramAsync(DossierGanttDiagramQuery dossierGanttDiagramQuery);
        Task<IEnumerable<Dossier>> GetDossiersAsync();
        Task<DossierLastDocumentResponse> RegisterDerivationAsync(DossierLastDocumentRequest dossierLastDocumentRequest, string userId);
        Task<Dossier> UpdateDossierForInitialRequestAsync(Dossier dossier);
        Task<DossierResponse> GetDossierByIdAsync(string id);
        Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber);
        Task<IEnumerable<UserRequestWithPublicDeedResponse>> GetUserRequestsWithPublicDeedAsync(string documentNumber);
    }
}
