using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.SolicitorDossier;
using SISGED.Shared.Models.Responses.SolicitorDossier;

namespace SISGED.Server.Services.Contracts
{
    public interface ISolicitorDossierService : IGenericService
    {
        Task<int> CountSolicitorDossiersAsync(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery);
        Task<IEnumerable<int>> GetSolicitorDossierAvailableYearsAsync(string solicitorId);
        Task<IEnumerable<SolicitorDossier>> GetSolicitorsDossiersAsync(SolicitorDossierPaginationQuery solicitorDossierPaginationQuery);
    }
}
