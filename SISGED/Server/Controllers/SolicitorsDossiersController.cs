using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Queries.SolicitorDossier;
using SISGED.Shared.Models.Responses.SolicitorDossier;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    
    public class SolicitorsDossiersController : ControllerBase
    {
        private readonly ISolicitorDossierService _solicitorDossierService;
        private readonly IMapper _mapper;

        public SolicitorsDossiersController(ISolicitorDossierService solicitorDossierService, IMapper mapper)
        {
            _solicitorDossierService = solicitorDossierService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedSolicitorDossierResponse>> GetSolicitorsDossiersAsync([FromQuery] SolicitorDossierPaginationQuery solicitorDossierPaginationQuery)
        {
            try
            {
                var solicitorDossiers = await _solicitorDossierService.GetSolicitorsDossiersAsync(solicitorDossierPaginationQuery);

                var solicitorDossiersResponse = _mapper.Map<IEnumerable<SolicitorDossierResponse>>(solicitorDossiers);

                var totalSolicitorDossiers = await _solicitorDossierService.CountSolicitorDossiersAsync(solicitorDossierPaginationQuery);

                var paginatedSolicitorDossierResponse = new PaginatedSolicitorDossierResponse(solicitorDossiersResponse, totalSolicitorDossiers);

                return Ok(paginatedSolicitorDossierResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{solicitorId}/years")]
        public async Task<ActionResult<IEnumerable<int>>> GetSolicitorDossierAvailableYearsAsync([FromRoute] string solicitorId)
        {
            try
            {
                var solicitorDossierYears = await _solicitorDossierService.GetSolicitorDossierAvailableYearsAsync(solicitorId);

                return Ok(solicitorDossierYears);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
