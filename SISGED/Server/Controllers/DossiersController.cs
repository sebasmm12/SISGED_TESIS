using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Repositories;
using SISGED.Shared.Models.Queries.Dossier;
using SISGED.Shared.Models.Queries.UserDocument;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.UserDocument;
using SISGED.Shared.Models.Responses.UserDossier;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DossiersController : ControllerBase
    {
        private readonly IDossierService _dossierService;
        private readonly IMapper _mapper;

        public DossiersController(IDossierService dossierService, IMapper mapper)
        {
            _dossierService = dossierService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DossierInfoResponse>>> GetDossiersAsync()
        {
            try
            {
                var dossiers = await _dossierService.GetDossiersAsync();

                return Ok(dossiers);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{dossierId}")]
        public async Task<ActionResult<DossierInfoResponse>> GetDossierAsync([FromRoute] string dossierId)
        {
            try
            {
                var dossier = await _dossierService.GetDossierAsync(dossierId);

                var dossierResponse = _mapper.Map<DossierInfoResponse>(dossier);

                return Ok(dossier);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("derivations/{userId}")]
        public async Task<ActionResult<DossierLastDocumentResponse>> RegisterDerivationAsync([FromBody] DossierLastDocumentRequest dossierLastDocumentRequest, [FromRoute] string userId)
        {
            try
            {
                var dossierLastDocument = await _dossierService.RegisterDerivationAsync(dossierLastDocumentRequest, userId);

                return Ok(dossierLastDocument);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<IEnumerable<DossierInfoResponse>>> GetFilteredDossiersAsync([FromQuery] DossierHistoryQuery dossierHistoryQuery)
        {
            try
            {
                var filteredDossiers = await _dossierService.GetDossierByFiltersAsync(dossierHistoryQuery);

                var dossiersResponse = _mapper.Map<IEnumerable<DossierInfoResponse>>(filteredDossiers);

                return Ok(dossiersResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<PaginatedUserDossierResponse>> GetDossierListAsync([FromQuery] UserDossierPaginationQuery userDossierPaginationQuery)
        {
            try
            {
                var dossiers = await _dossierService.GetDossiersListAsync(userDossierPaginationQuery);

                var totalDossiers = await _dossierService.CountDossiersListAsync(userDossierPaginationQuery);

                var paginatedDocumentsResponse = new PaginatedUserDossierResponse(dossiers, totalDossiers);

                return Ok(paginatedDocumentsResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
