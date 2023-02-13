using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentVersion;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DocumentVersionsController : ControllerBase
    {
        private readonly IDocumentVersionService _documentVersionService;
        private readonly IMapper _mapper;

        public DocumentVersionsController(IDocumentVersionService documentVersionService, IMapper mapper)
        {
            _documentVersionService = documentVersionService;
            _mapper = mapper;
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<IEnumerable<DocumentVersionInfo>>> GetContentVersionsByDocumentIdAsync([FromRoute] string documentId)
        {
            try
            {
                var contentVersions = await _documentVersionService.GetContentVersionsByDocumentIdAsync(documentId);

                var contentVersionsResponse = _mapper.Map<IEnumerable<DocumentVersionInfo>>(contentVersions);

                return Ok(contentVersions);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
