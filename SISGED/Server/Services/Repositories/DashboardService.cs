using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Factories.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Repositories;

public class DashboardService : IDashboardService
{
    private readonly ITrayService _trayService;
    private readonly IDossierService _dossierService;
    private readonly IRoleDocumentsFactory _roleDocumentsFactory;

    public DashboardService(ITrayService trayService, IDossierService dossierService, IRoleDocumentsFactory roleDocumentsFactory)
    {
        _trayService = trayService;
        _dossierService = dossierService;
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

    public async Task<UserTraysSnapshotResponse> GetUserTraysSnapshot(string userId)
    {
        var userTraysSnapshot = await _trayService.GetUserTraysSnapshotAsync(userId);

        return userTraysSnapshot;
    }
    public async Task<IEnumerable<DossierSnapshotResponse>> GetDossiersSnapshot()
    {
        var dossiersSnapshot = await _dossierService.GetDossiersSnapshotAsync();

        return dossiersSnapshot;
    }
}