using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Dossier;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DossiersController: ControllerBase
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

        [HttpPost("derivations/{userId}")]
        public async Task<ActionResult<IEnumerable<DossierLastDocumentResponse>>> RegisterDerivationAsync([FromBody] DossierLastDocumentRequest dossierLastDocumentRequest, [FromRoute] string userId)
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
    }
}
