using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Queries.UserDocument;
using SISGED.Shared.Models.Responses.UserDocument;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/users")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class UsersDocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public UsersDocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{userId}/documents")]
        public async Task<ActionResult<PaginatedUserDocumentResponse>> GetDocumentsAsync([FromRoute] string userId, [FromQuery] UserDocumentPaginationQuery userDocumentPaginationQuery)
        {
            try
            {
                var documentsByUser = await _documentService.GetDocumentsByUserAsync(userId, userDocumentPaginationQuery);

                var totalDocuments = 0;
                    
                if(documentsByUser.Any())
                    totalDocuments = await _documentService.CountDocumentsByUserAsync(userId, userDocumentPaginationQuery);

                var paginatedDocumentsResponse = new PaginatedUserDocumentResponse(documentsByUser, totalDocuments);

                return Ok(paginatedDocumentsResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
