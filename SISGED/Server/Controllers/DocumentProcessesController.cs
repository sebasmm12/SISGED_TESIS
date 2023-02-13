using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.DocumentProcess;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DocumentProcessesController : ControllerBase
    {
        private readonly IDocumentProcessService _documentProcessService;

        public DocumentProcessesController(IDocumentProcessService documentProcessService)
        {
            _documentProcessService = documentProcessService;
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<IEnumerable<DocumentProcessInfo>>> GetProcessesByDocumentIdAsync([FromRoute] string documentId)
        {
            try
            {
                var processes = await _documentProcessService.GetProcessesByDocumentIdAsync(documentId);

                return Ok(processes);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
