using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.DocumentEvaluation;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DocumentEvaluationsController : ControllerBase
    {
        private readonly IDocumentEvaluationService _documentProcessService;

        public DocumentEvaluationsController(IDocumentEvaluationService documentEvaluationService)
        {
            _documentProcessService = documentEvaluationService;
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<IEnumerable<DocumentEvaluationInfo>>> GetProcessesByDocumentIdAsync([FromRoute] string documentId)
        {
            try
            {
                var evaluations = await _documentProcessService.GetEvaluationsByDocumentIdAsync(documentId);

                return Ok(evaluations);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
