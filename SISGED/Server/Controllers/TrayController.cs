using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        private readonly IMapper _mapper;

        public TrayController(ITrayService trayService,IMapper mapper)
        {
            _trayService = trayService;
            _mapper = mapper;
        }

        [HttpGet("{usuario}")]
        public async Task<ActionResult<InputOutputTrayResponse>> GetAsync(string user)
        {
            var tray = await _trayService.GetAsync(user);
            return Ok(tray);
        }

        [HttpGet("bandejaentrada/{usuario}")]
        public async Task<ActionResult<List<InputTrayResponse>>> GetInputStrayAsync(string user)
        {
            var tray = await _trayService.GetInputStrayAsync(user);
            return Ok(tray);
        }
    }
}
