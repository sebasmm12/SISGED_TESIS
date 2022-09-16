using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.Appeal;
using SISGED.Shared.Models.Responses.Document.BPNRequest;
using SISGED.Shared.Models.Responses.Document.Dictum;
using SISGED.Shared.Models.Responses.Document.InitialRequest;
using SISGED.Shared.Models.Responses.Document.Resolution;
using SISGED.Shared.Models.Responses.Document.UserRequest;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;

        public DocumentsController(IDocumentService documentService, IMapper mapper)
        {
            _documentService = documentService;
            _mapper = mapper;
        }

        #region GET Services
        [HttpGet("userRequests/{documentNumber}")]
        public async Task<ActionResult<IEnumerable<UserRequestDocumentResponse>>> GetUserRequestDocumentsAsync([FromRoute] string documentNumber)
        {
            try
            {
                var userRequestDocuments = await _documentService.GetUserRequestDocumentsAsync(documentNumber);

                return Ok(userRequestDocuments);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("userRequests-publicDeeds/{documentNumber}")]
        public async Task<ActionResult<IEnumerable<UserRequestWithPublicDeedResponse>>> GetUserRequestsWithPublicDeedAsync([FromRoute] string documentNumber)
        {
            try
            {
                var usersRequestWithPublicDeed = await _documentService.GetUserRequestsWithPublicDeedAsync(documentNumber);

                return Ok(usersRequestWithPublicDeed);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("bpnRequests/{documentId}")]
        public async Task<ActionResult<BPNRequestInfoResponse>> GetBPNRequestDocumentAsync([FromRoute] string documentId)
        {
            try
            {
                var bpnRequest = await _documentService.GetBPNRequestDocumentAsync(documentId);

                var bpnRequestResponse = _mapper.Map<BPNRequestInfoResponse>(bpnRequest);

                return Ok(bpnRequestResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("dictums/{documentId}")]
        public async Task<ActionResult<DictumInfoResponse>> GetDictumDocumentAsync([FromRoute] string documentId)
        {
            try
            {
                var dictum = await _documentService.GetDictumDocumentAsync(documentId);

                var dictumResponse = _mapper.Map<DictumInfoResponse>(dictum);

                return Ok(dictumResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("resolutions/{documentId}")]
        public async Task<ActionResult<ResolutionInfoResponse>> GetResolutionDocumentAsync([FromRoute] string documentId)
        {
            try
            {
                var resolution = await _documentService.GetResolutionDocumentAsync(documentId);

                var resolutionResponse = _mapper.Map<ResolutionInfoResponse>(resolution);

                return Ok(resolutionResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("appeals/{documentId}")]
        public async Task<ActionResult<AppealInfoResponse>> GetAppealDocumentAsync([FromRoute] string documentId)
        {
            try
            {
                var appeal = await _documentService.GetAppealDocumentAsync(documentId);

                var appealResponse = _mapper.Map<AppealInfoResponse>(appeal);

                return Ok(appealResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{documentId}")]
        public async Task<ActionResult<DocumentResponse>> GetDocumentAsync([FromRoute] string documentId)
        {
            try
            {
                var document = await _documentService.GetDocumentAsync(documentId);

                return Ok(document);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("initialRequest/{documentId}")]
        public async Task<ActionResult<InitialRequestInfoResponse>> GetInitialRequestDocumentAsync([FromRoute] string documentId)
        {
            try
            {
                var initialRequest = await _documentService.GetInitialRequestDocumentAsync(documentId);

                var initialRequestResponse = _mapper.Map<InitialRequestInfoResponse>(initialRequest);

                return Ok(initialRequestResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
            
        
        #endregion        

        #region PUT Services
        #endregion

        #region POST Services
        #endregion

    }
}
