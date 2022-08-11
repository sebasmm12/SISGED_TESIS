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
        public async Task<ActionResult<InputOutputTrayResponse>> GetAsync(string usuario)
        {
            //return await _trayService.ObtenerBandeja(usuario);
            return NoContent();
        }

        [HttpGet("bandejaentrada/{usuario}")]
        public async Task<ActionResult<List<InputTrayResponse>>> GetInputStrayAsync(string usuario)
        {
            //return await _trayService.ObtenerBandejaEntrada(usuario);
            return NoContent();
        }
    }
}
