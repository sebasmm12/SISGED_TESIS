using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class TrayController : ControllerBase
    {
        private readonly ITrayService _trayService;

        public TrayController(ITrayService trayService, IMapper mapper)
        {
            _trayService = trayService;
        }

        [HttpGet("{user}")]
        public async Task<ActionResult<InputOutputTrayResponse>> GetAsync(string user)
        {
            try
            {
                var tray = await _trayService.GetAsync(user);
                return Ok(tray);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("bandejaentrada/{user}")]
        public async Task<ActionResult<List<InputTrayResponse>>> GetInputStrayAsync(string user)
        {
            var tray = await _trayService.GetInputStrayAsync(user);
            return Ok(tray);
        }
    }
}
