using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Contracts;

public interface IDashboardService
{
    Task<ExpiredTraysDocumentsResponse> GetExpiredTraysDocuments(string userId);

    Task<IEnumerable<RoleDocumentsResponse>> GetRolesDocumentsAsync(DateFilterDTO dateFilter);
}