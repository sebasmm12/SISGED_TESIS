using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Factories.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Repositories;

public class DashboardService : IDashboardService
{
    private readonly ITrayService _trayService;
    private readonly IRoleDocumentsFactory _roleDocumentsFactory;

    public DashboardService(ITrayService trayService, IRoleDocumentsFactory roleDocumentsFactory)
    {
        _trayService = trayService;
        _roleDocumentsFactory = roleDocumentsFactory;
    }

    public async Task<ExpiredTraysDocumentsResponse> GetExpiredTraysDocuments(string userId)
    {
        var expiredInputTrayDocumentsTask =  _trayService.GetNextExpiredTraysDocumentsAsync(userId, "inputTray");

        var expiredOutputTrayDocumentsTask = _trayService.GetNextExpiredTraysDocumentsAsync(userId, "outputTray");

        await Task.WhenAll(expiredInputTrayDocumentsTask, expiredOutputTrayDocumentsTask);

        var expiredInputTrayDocuments = await expiredInputTrayDocumentsTask;
        var expiredOutputTrayDocuments = await expiredOutputTrayDocumentsTask;

        return new()
        {
            ExpiredInputTrayDocuments = expiredInputTrayDocuments,
            ExpiredOutputTrayDocuments = expiredOutputTrayDocuments
        };
    }

    public async Task<IEnumerable<RoleDocumentsResponse>> GetRolesDocumentsAsync(DateFilterDTO dateFilter)
    {
        var rolesDocuments = await _roleDocumentsFactory.GetRolesDocumentsAsync(dateFilter);

        return rolesDocuments;
    }
}