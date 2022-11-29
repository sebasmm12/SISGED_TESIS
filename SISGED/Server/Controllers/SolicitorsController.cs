using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class SolicitorsController : ControllerBase
    {
        private readonly ISolicitorService _solicitorService;
        private readonly IMapper _mapper;

        public SolicitorsController(ISolicitorService solicitorService, IMapper mapper)
        {
            _solicitorService = solicitorService;
            _mapper = mapper;
        }

        [HttpGet("{solicitorId}")]
        public async Task<ActionResult<SolicitorInfoResponse>> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitor = await _solicitorService.GetSolicitorByIdAsync(solicitorId);
                var solicitorResponse = _mapper.Map<SolicitorInfoResponse>(solicitor);

                return Ok(solicitorResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("autocomplete")]
        public async Task<ActionResult<IEnumerable<SolicitorInfoResponse>>> GetAutocompletedSolicitorsAsync([FromQuery] string? solicitorName,
            [FromQuery] bool? exSolicitor)
        {
            try
            {
                var solicitors = await _solicitorService.GetAutocompletedSolicitorsAsync(solicitorName, exSolicitor);
                var solicitorsResponse = _mapper.Map<IEnumerable<SolicitorInfoResponse>>(solicitors);

                return Ok(solicitorsResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
