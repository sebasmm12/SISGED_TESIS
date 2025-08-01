using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;

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
}