using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Queries.Dashboard;

namespace SISGED.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class DashboardsController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardsController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }


    [HttpGet("expired-trays-documents/{userId}")]
    public async Task<IActionResult> GetExpiredTraysDocumentsAsync(string userId)
    {
        try
        {
            var expiredTraysDocuments = await _dashboardService.GetExpiredTraysDocuments(userId);

            return Ok(expiredTraysDocuments);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("roles/documents/{dateFilter}")]
    public async Task<IActionResult> GetRolesDocumentsAsync([FromRoute] DateFilterDTO dateFilter)
    {
        try
        {
            var rolesDocuments = await _dashboardService.GetRolesDocumentsAsync(dateFilter);

            return Ok(rolesDocuments);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("user-trays-snapshot/{userId}")]
    public async Task<IActionResult> GetUserTraysSnapshotAsync(string userId)
    {
        try
        {
            var userTraysSnapshot = await _dashboardService.GetUserTraysSnapshotAsync(userId);

            return Ok(userTraysSnapshot);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("user-documents-snapshot/{userId}")]
    public async Task<IActionResult> GetUserDocumentsSnapshotAsync(string userId)
    {
        try
        {
            var userDocumentsSnapshot = await _dashboardService.GetUserDocumentsSnapshotAsync(userId);

            return Ok(userDocumentsSnapshot);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("dossiers-snapshot")]
    public async Task<IActionResult> GetDossiersSnapshotAsync()
    {
        try
        {
            var dossiersSnapshot = await _dashboardService.GetDossiersSnapshotAsync();

            return Ok(dossiersSnapshot);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpGet("user-document-history-state")]
    public async Task<IActionResult> GetUserDocumentHistoryStateAsync([FromQuery] UserDocumentTypeRequestQuery userDocumentTypeRequestQuery)
    {
        try
        {
            var userTypeDocuments = await _dashboardService.GetUserDocumentHistoryStateAsync(userDocumentTypeRequestQuery.UserId,userDocumentTypeRequestQuery.DateFilter);

            return Ok(userTypeDocuments);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}