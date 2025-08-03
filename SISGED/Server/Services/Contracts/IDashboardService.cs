using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Contracts;

public interface IDashboardService
{
    Task<ExpiredTraysDocumentsResponse> GetExpiredTraysDocuments(string userId);

    Task<IEnumerable<RoleDocumentsResponse>> GetRolesDocumentsAsync(DateFilterDTO dateFilter);
    Task<UserTraysSnapshotResponse> GetUserTraysSnapshotAsync(string userId);
    Task<UserDocumentSnapshotResponse> GetUserDocumentsSnapshotAsync(string userId);
    Task<IEnumerable<DossierSnapshotResponse>> GetDossiersSnapshotAsync();
    Task<IEnumerable<UserDocumentHistoryStateResponse>> GetUserDocumentHistoryStateAsync(string clientId, DateFilterDTO dateFilter);

}