using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Factories.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Server.Services.Repositories;

public class DashboardService : IDashboardService
{
    private readonly ITrayService _trayService;
    private readonly IDossierService _dossierService;
    private readonly IDocumentService _documentService;
    private readonly IRoleDocumentsFactory _roleDocumentsFactory;
    private readonly IUserHistoryDocumentStateFactory _userHistoryDocumentStateFactory;

    public DashboardService(ITrayService trayService, IDossierService dossierService, IDocumentService documentService, IRoleDocumentsFactory roleDocumentsFactory, IUserHistoryDocumentStateFactory userHistoryDocumentStateFactory)
    {
        _trayService = trayService;
        _dossierService = dossierService;
        _documentService = documentService;
        _roleDocumentsFactory = roleDocumentsFactory;
        _userHistoryDocumentStateFactory = userHistoryDocumentStateFactory;
    }

    public async Task<ExpiredTraysDocumentsResponse> GetExpiredTraysDocuments(string userId)
    {
        var expiredTrayDocuments =  await _trayService.GetNextExpiredTraysDocumentsAsync(userId);

        return new(expiredTrayDocuments);
    }

    public async Task<IEnumerable<RoleDocumentsResponse>> GetRolesDocumentsAsync(DateFilterDTO dateFilter)
    {
        var rolesDocuments = await _roleDocumentsFactory.GetRolesDocumentsAsync(dateFilter);

        return rolesDocuments;
    }

    public async Task<UserTraysSnapshotResponse> GetUserTraysSnapshotAsync(string userId)
    {
        var userTraysSnapshot = await _trayService.GetUserTraysSnapshotAsync(userId);

        return userTraysSnapshot;
    }

    public Task<UserDocumentSnapshotResponse> GetUserDocumentsSnapshotAsync(string userId)
    {
        var userDocumentsSnapshot = _documentService.GetUserDocumentsSnapshotAsync(userId);

        return userDocumentsSnapshot;
    }
    public async Task<IEnumerable<DossierSnapshotResponse>> GetDossiersSnapshotAsync()
    {
        var dossiersSnapshot = await _dossierService.GetDossiersSnapshotAsync();

        return dossiersSnapshot;
    }

    public async Task<IEnumerable<UserDocumentHistoryStateResponse>> GetUserDocumentHistoryStateAsync(string userId, DateFilterDTO dateFilter)
    {
        var userDocumentHistoryState = await _userHistoryDocumentStateFactory.GetUserHistoryDocumentStateAsync(userId, dateFilter);

        return userDocumentHistoryState;
    }
}