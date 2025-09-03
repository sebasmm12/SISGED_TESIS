using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;

namespace SISGED.Server.Controllers;


[ApiController]
[Route("api/documents/{documentId}/annexes")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class DocumentAnnexesController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly IDocumentService _documentService;

    private readonly string _containerName = "solicitudesiniciales";
    private readonly string _generatedContainerName = "documentosgenerados";

    public DocumentAnnexesController(
        IMediaService mediaService, 
        IDocumentService documentService)
    {
        _mediaService = mediaService;
        _documentService = documentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromRoute]string documentId)
    {
        try
        {
            var document = await _documentService.GetDocumentAsync(documentId);

            var annexes = await _mediaService.GetFilesAsync(document.AttachedUrls, _containerName);

            return Ok(annexes);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
    }
}