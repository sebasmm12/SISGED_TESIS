using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.DocumentType;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DocumentTypesController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IMapper _mapper;

        public DocumentTypesController(IDocumentTypeService documentTypeService, IMapper mapper)
        {
            _documentTypeService = documentTypeService;
            _mapper = mapper;
        }

        [HttpGet("{type}")]
        public async Task<ActionResult<IEnumerable<DocumentTypeInfoResponse>>> GetDocumentTypesAsync(string type)
        {
            try
            {
                var documentTypes = await _documentTypeService.GetDocumentTypesAsync(type);

                var documentTypesResponse = _mapper.Map<IEnumerable<DocumentTypeInfoResponse>>(documentTypes);

                return Ok(documentTypes);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }
    }
}
