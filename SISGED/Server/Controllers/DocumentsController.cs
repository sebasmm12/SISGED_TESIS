using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Document;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.Appeal;
using SISGED.Shared.Models.Responses.Document.BPNDocument;
using SISGED.Shared.Models.Responses.Document.BPNRequest;
using SISGED.Shared.Models.Responses.Document.Dictum;
using SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness;
using SISGED.Shared.Models.Responses.Document.InitialRequest;
using SISGED.Shared.Models.Responses.Document.Resolution;
using SISGED.Shared.Models.Responses.Document.SignConclusion;
using SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.DossierDocument;
using System.Net.WebSockets;
using Json = System.Text.Json;

namespace SISGED.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IDossierService _dossierService;
        private readonly ITrayService _trayService;
        private readonly IPublicDeedsService _publicdeedsService;
        private readonly IFileStorageService _fileService;
        private readonly IAssistantService _assistantService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        private readonly IMediaService _mediaService;

        private readonly string _containerName = "solicitudesiniciales";
        private readonly string _generatedContainerName = "documentosgenerados";

        public DocumentsController(IDocumentService documentService, IDossierService dossierService, ITrayService trayService,
            IPublicDeedsService publicDeedsService, IFileStorageService fileStorage, IAssistantService assistantService, IMapper mapper,
            IMediaService mediaService, IUserService userService)
        {
            _documentService = documentService;
            _dossierService = dossierService;
            _trayService = trayService;
            _publicdeedsService = publicDeedsService;
            _fileService = fileStorage;
            _assistantService = assistantService;
            _mapper = mapper;
            _mediaService = mediaService;
            _userService = userService;
        }

        #region POST

        [HttpPost("documentoodn")]
        public async Task<ActionResult<SolicitorDesignationDocument>> SolicitorDesignationOfficeRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                SolicitorDesignationDocumentResponse sdd = new SolicitorDesignationDocumentResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                sdd = JsonConvert.DeserializeObject<SolicitorDesignationDocumentResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in sdd.content.URLannex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "oficiodesignacionnotario");

                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                SolicitorDesignationDocument documentoODN = new SolicitorDesignationDocument();
                documentoODN = await _documentService.SolicitorDesignationOfficeRegisterAsync(dossierWrapper, url2);
                return Ok(documentoODN);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentosbpn")]
        public async Task<ActionResult<BPNDocument>> OfficeBPNDocumentRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                BPNOfficeResponse oficioBPNDTO = new BPNOfficeResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                oficioBPNDTO = JsonConvert.DeserializeObject<BPNOfficeResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in oficioBPNDTO.Content.URLannex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "oficiobpn");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;

                        url2.Add(urlData2);
                    }
                }
                string urlData = "";
                if (!string.IsNullOrWhiteSpace(oficioBPNDTO.Content.Data))
                {
                    var solicitudBytes = Convert.FromBase64String(oficioBPNDTO.Content.Data);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes, "pdf", "oficiobpn");
                    urlData = await _fileService.SaveFileAsync(file) ?? string.Empty;
                }
                BPNDocument documentoOficioBPN = new BPNDocument();
                documentoOficioBPN = await _documentService.RegisterBPNOfficeAsync(dossierWrapper, url2, urlData);
                return Ok(documentoOficioBPN);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentosolicbpn")]
        public async Task<ActionResult<BPNRequest>> BPNRequestDocumentRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                //Obtenemos los datos del expedientewrapper
                BPNRequestResponse document = new BPNRequestResponse();
                BPNRequestResponseContent grantorlist = new BPNRequestResponseContent();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                document = JsonConvert.DeserializeObject<BPNRequestResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";

                foreach (string u in document!.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "solicitudbpn");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;

                        url2.Add(urlData2);
                    }
                }

                BPNRequest solicitudBPN = new BPNRequest();
                solicitudBPN = await _documentService.RegisterBPNRquestAsync(dossierWrapper, url2);

                return Ok(solicitudBPN);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("complaint-requests")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ComplaintRequest>> ComplaintRequestDocumentRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                var document = DeserializeDocument<ComplaintRequestResponse>(dossierWrapper.Document);

                var user = await GetUserAsync("userId");

                var complaintRequest = await RegisterComplaintRequestAsync(document, user);

                var dossier = await UpdateDossierByDocumentAsync(new(dossierWrapper, "Denuncia" , "En proceso"
                            , new(2, complaintRequest.Id, complaintRequest.Type, DateTime.UtcNow.AddHours(-5).AddDays(10))));

                await RegisterOutPutTrayAsync(complaintRequest, user, dossier);

                return Ok(complaintRequest);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentosef")]
        //public async Task<ActionResult<ExpedienteBandejaDTO>> RegistrarDocumentoSEF(ExpedienteWrapper expedientewrapper)
        public async Task<ActionResult<SignExpeditionRequest>> RegisterSEFDocument(DossierWrapper dossierWrapper)
        {
            try
            {
                //Conversion de Obj a tipo SolicitudExpedicionFirmaDTO
                SignExpeditionRegisterResponse solicitudExpedicionFirmasDTO = new SignExpeditionRegisterResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                solicitudExpedicionFirmasDTO = JsonConvert.DeserializeObject<SignExpeditionRegisterResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in solicitudExpedicionFirmasDTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "solicitudexpedicionfirma");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                //Almacenamiento de archivo en repositorio y obtnecion de url
                string urlData = "";
                if (!string.IsNullOrWhiteSpace(solicitudExpedicionFirmasDTO.Content.Data))
                {
                    var solicitudBytes = Convert.FromBase64String(solicitudExpedicionFirmasDTO.Content.Data);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes, "pdf", "solicitudexpedicionfirma");
                    urlData = await _fileService.SaveFileAsync(file) ?? string.Empty;
                }

                SignExpeditionRequest documentoSEF = new SignExpeditionRequest();
                documentoSEF = await _documentService.RegisterSignExpeditionRequestAsync(dossierWrapper, url2, urlData);

                return Ok(documentoSEF);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("user-requests")]
        public async Task<ActionResult<DossierDocumentInitialRequestResponse>> InitialRequestDocumentRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                var document = DeserializeDocument<InitialRequestResponse>(dossierWrapper.Document);

                var user = await GetUserAsync("userId");
                //var user = await _userService.GetUserByIdAsync("5ef9f9c1afdbc540d868b3ce");

                var initialRequest = await RegisterInitialRequestAsync(document, user);

                var dossier = await RegisterInitialDossierAsync(document, initialRequest);

                string receiveUserId = await _trayService.RegisterUserInputTrayAsync(dossier.Id, initialRequest.Id, "MesaPartes");

                await _documentService.UpdateDocumentProcessAsync(new(user.Id, receiveUserId, "derivado", user.Rol), initialRequest.Id);

                // TODO: Implement the assistant service when creating the initial request
                //var assistant = new Assistant();
                //assistant.DossierId = dossier.Id;
                //assistant.Steps = new();
                //assistant.Steps.DossierName = "Solicitud";

                //await _assistantService.CreateAsync(assistant);

                var dossierDocumentResponse = new DossierDocumentInitialRequestResponse(dossier, initialRequest);

                return Ok(dossierDocumentResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentocf")]
        public async Task<ActionResult<SignConclusion>> CFDocumentRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                SignConclusionResponse conclusionfirmaDTO = new SignConclusionResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                conclusionfirmaDTO = JsonConvert.DeserializeObject<SignConclusionResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in conclusionfirmaDTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "conclusionfirma");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }

                DossierResponse expedientePorConsultar = await _dossierService.GetDossierByIdAsync(dossierWrapper.Id);
                DossierDocument documentosolicitud = expedientePorConsultar.Documents.Find(x => x.Type == "SolicitudInicial")!;

                SignConclusion documentoCF = new SignConclusion();
                documentoCF = await _documentService.singConclusionERegisterAsync(dossierWrapper, url2, documentosolicitud.DocumentId);
                await _publicdeedsService.updatePublicDeedBySignatureConclusionAsync(conclusionfirmaDTO.Content.PublicDeedId);
                return Ok(documentoCF);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("disciplinary-openness")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<DisciplinaryOpenness>> DisciplinaryOpennessDocumentRegister([FromBody]DossierWrapper dossierWrapper)
        {
            try
            {
                var document = DeserializeDocument<DisciplinaryOpennessResponse>(dossierWrapper.Document);

                var user = await GetUserAsync("userId");

                var disciplinaryOpenness = await RegisterDisciplinaryOpennessDocumentAsync(document, user);

                var updatedDossier = await UpdateDossierByDocumentAsync(new(dossierWrapper, "Denuncia", "En proceso", 
                    new(3, disciplinaryOpenness.Id, disciplinaryOpenness.Type, DateTime.UtcNow.AddHours(-5).AddDays(5))));

                await RegisterOutPutTrayAsync(disciplinaryOpenness, user, updatedDossier);

                return Ok(disciplinaryOpenness);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("solicitor-dossier-request")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SolicitorDossierRequest>> SolicitorDossierRequestRegister([FromBody] DossierWrapper dossierWrapper)
        {
            try
            {
                var document = DeserializeDocument<SolicitorDossierRequestResponse>(dossierWrapper.Document);

                var user = await GetUserAsync("userId");

                var solicitorDossierRequest = await RegisterSolicitorDossierRequestDocumentAsync(document, user);

                var updatedDossier = await UpdateDossierByDocumentAsync(new(dossierWrapper, "Denuncia" , "En proceso", 
                    new(4, solicitorDossierRequest.Id, solicitorDossierRequest.Type, DateTime.UtcNow.AddHours(-5).AddDays(5))));

                await RegisterOutPutTrayAsync(solicitorDossierRequest, user, updatedDossier);

                return Ok(solicitorDossierRequest);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("dictums")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Dictum>> RegisterDictumAsync(DossierWrapper dossierWrapper)
        {
            try
            {
                var document = DeserializeDocument<DictumResponse>(dossierWrapper.Document);

                var user = await GetUserAsync("userId");

                var dictum = await RegisterDictumAsync(document, user);

                var dossier = await UpdateDossierByDocumentAsync(new(dossierWrapper, "Denuncia", "En proceso",
                    new(6, dictum.Id, "Dictamen", DateTime.UtcNow.AddHours(-5).AddDays(5))));

                await RegisterOutPutTrayAsync(dictum, user, dossier);

                return Ok(dictum);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("resolutions")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Resolution>> ResolutionDocumentRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                var document = DeserializeDocument<ResolutionResponse>(dossierWrapper.Document);

                var user = await GetUserAsync("userId");

                var resolutionDocument = await RegisterResolutionDocumentAsync(document, user);

                var updatedDossier = await UpdateDossierByDocumentAsync(new(dossierWrapper, "Denuncia", "Finalizado",
                    new(7, resolutionDocument.Id, resolutionDocument.Type, DateTime.UtcNow.AddHours(-5).AddDays(5))));

                await RegisterOutPutTrayAsync(resolutionDocument, user, updatedDossier);

                return Ok(resolutionDocument);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentoResultadoBPN")]
        public async Task<ActionResult<BPNResult>> BPNResultDocumentRegister(DossierWrapper dossierWrapper)
        {
            try
            {
                BPNResultResponse DTO = new BPNResultResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<BPNResultResponse>(json)!;

                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "resultadobpn");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }

                DossierResponse expedientePorConsultar = await _dossierService.GetDossierByIdAsync(dossierWrapper.Id);
                DossierDocument documentRequest = expedientePorConsultar.Documents.Find(x => x.Type == "SolicitudInicial")!;

                var result = await _documentService.BPNResultRegisterAsync(DTO, url2, dossierWrapper.CurrentUserId, dossierWrapper.Id, dossierWrapper.InputDocument, documentRequest.DocumentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("solicitor-dossier-shipments")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<SolicitorDossierShipment>> RegisterSolicitorDossierShipmentAsync(DossierWrapper dossierWrapper)
        {
            try
            {
                var document = DeserializeDocument<SolicitorDossierShipmentResponse>(dossierWrapper.Document);

                var user = await GetUserAsync("userId");

                var solicitorDossierShipment = await RegisterSolicitorDossierShipmentAsync(document, user);

                var dossier = await UpdateDossierByDocumentAsync(new(dossierWrapper, "Denuncia", "En proceso",
                        new(5, solicitorDossierShipment.Id, "EntregaExpedienteNotario", DateTime.UtcNow.AddHours(-5).AddDays(5))));

                await RegisterOutPutTrayAsync(solicitorDossierShipment, user, dossier);

                return Ok(solicitorDossierShipment);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }

        #endregion

        #region PUT
        [HttpPut("evaluation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Document>> EvaluateDocument(DocumentEvaluationRequest documentEvaluationRequest)
        {
            try
            {
                var user = await GetUserAsync("userId");
                var state = await _documentService.EvaluateDocumentAsync(documentEvaluationRequest, user);

                return Ok(state);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("generation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Document>> GenerateDocument(GenerateDocumentRequest documentRequest)
        {
            try
            {
                var user = await GetUserAsync("userId");

                var generatedDocument = await GetGeneratedDocumentAsync(documentRequest, user);
                
                var document = await _documentService.GenerateDocumentAsync(generatedDocument);
                
                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("cambiarestadodocumento")]
        public async Task<ActionResult<Document>> ModifySateDocument(DocumentRequest document)
        {
            try
            {
                var state = await _documentService.ModifyStateDocumentAsync(document);
                return Ok(state);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoODN")]
        public async Task<ActionResult<SolicitorDesignationDocument>> ModifyDocumentODN(DossierWrapper dossierWrapper)
        {
            try
            {
                SolicitorDesignationDocumentResponse DTO = new SolicitorDesignationDocumentResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<SolicitorDesignationDocumentResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.content.URLannex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "oficiodesignacionnotario");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                var update = await _documentService.UpdateDocumentODNAsync(dossierWrapper, url2);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoAPE")]
        public async Task<ActionResult<Appeal>> AppealDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                AppealResponse DTO = new AppealResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<AppealResponse>(json)!;
                string urlData = "";
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "apelaciones");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                if (!string.IsNullOrWhiteSpace(DTO.Content.Data))
                {
                    var solicitudBytes = Convert.FromBase64String(DTO.Content.Data);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes, "pdf", "apelaciones");
                    urlData = await _fileService.SaveFileAsync(file) ?? string.Empty;
                }
                var appeal = await _documentService.AppealDocumentUpdateAsync(dossierWrapper, urlData, url2);
                return Ok(appeal);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoAD")]
        public async Task<ActionResult<DisciplinaryOpenness>> DisciplinaryOpennessDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                DisciplinaryOpennessResponse DTO = new DisciplinaryOpennessResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<DisciplinaryOpennessResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (MediaRegisterDTO u in DTO.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u.Content))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u.Content);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "aperturamientodiciplinario");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                var openness = await _documentService.DisciplinaryOpennessDocumentUpdateAsync(dossierWrapper, url2);
                return openness;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoCF")]
        public async Task<ActionResult<SignConclusion>> SignConclusionDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                SignConclusionResponse DTO = new SignConclusionResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<SignConclusionResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "dictamen");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                var doc = await _documentService.UpdateSignConclusionDocumentAsync(dossierWrapper, url2);
                return Ok(doc);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoD")]
        public async Task<ActionResult<Dictum>> DictumDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                DictumResponse DTO = new DictumResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<DictumResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                //foreach (string u in DTO.Content.URLAnnex)
                //{
                //    if (!string.IsNullOrWhiteSpace(u))
                //    {
                //        var solicitudBytes2 = Convert.FromBase64String(u);
                //        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "dictamen");
                //        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                //        url2.Add(urlData2);
                //    }
                //}

                return await _documentService.UpdateDictumDocumentAsync(dossierWrapper, url2);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoOficioBPN")]
        public async Task<ActionResult<BPNDocument>> BPNOfficeDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                BPNOfficeResponse DTO = new BPNOfficeResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<BPNOfficeResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLannex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "oficiobpn");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                var update = await _documentService.UpdateBPNOfficeDocumentAsync(dossierWrapper, url2);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoR")]
        public async Task<ActionResult<Resolution>> ResolutionDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                ResolutionResponse DTO = new ResolutionResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<ResolutionResponse>(json)!;
                string urlData = "";
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (MediaRegisterDTO u in DTO.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u.Content))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u.Content);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "resolucion");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                if (!string.IsNullOrWhiteSpace(DTO.Content.Data))
                {
                    var solicitudBytes = Convert.FromBase64String(DTO.Content.Data);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes, "pdf", "resolucion");
                    urlData = await _fileService.SaveFileAsync(file) ?? string.Empty;
                }

                var update = await _documentService.UpdateResolutionDocumentAsync(dossierWrapper, urlData, url2);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoSEN")]
        public async Task<ActionResult> SENDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                await _documentService.UpdateSENDocumentAsync(dossierWrapper);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoResultadoBPN")]
        public async Task<ActionResult<BPNResult>> BPNResultDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                BPNResultResponse DTO = new BPNResultResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<BPNResultResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "resultadobpn");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                var doc = await _documentService.UpdateBPNResultDocumentAsync(dossierWrapper, url2);
                return Ok(doc);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoSolicitudInicial")]
        public async Task<ActionResult> InitialRequestDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                await _documentService.UpdateInitialRequestDocumentAsync(dossierWrapper);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("actualizarDocumentoEEN")]
        public async Task<ActionResult> EENDocumentModify(DossierWrapper dossierWrapper)
        {
            try
            {
                await _documentService.UpdateEENDocumentAsync(dossierWrapper);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("estadosolicitud")]
        public async Task<ActionResult> InitialRequestStateModify(DossierWrapper dossierWrapper)
        {
            try
            {
                await _documentService.UpdateInitialRequestStateAsync(dossierWrapper);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region GET Services
        [HttpGet("user-requests/{documentNumber}")]
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

        [HttpGet("user-requests-public-deeds")]
        public async Task<ActionResult<IEnumerable<UserRequestWithPublicDeedResponse>>> GetUserRequestsWithPublicDeedAsync([FromQuery] UserRequestPaginationQuery userRequestPaginationQuery)
        {
            try
            {
                var usersRequestWithPublicDeed = await _documentService.GetUserRequestsWithPublicDeedAsync(userRequestPaginationQuery);

                long totalUserRequests = await _documentService.CountUserRequestAsync(userRequestPaginationQuery.DocumentNumber);

                var paginatedUserRequests = new PaginatedUserRequest(usersRequestWithPublicDeed, totalUserRequests);

                return Ok(paginatedUserRequests);
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

        [HttpGet("initialRequests/{documentId}")]
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

        [HttpGet("bpnDocuments/{documentId}")]
        public async Task<ActionResult<BPNDocumentInfoResponse>> GetBPNDocumentAsync([FromRoute] string documentId)
        {
            try
            {
                var bpnDocumentResponse = await _documentService.GetBPNDocumentAsync(documentId);

                return Ok(bpnDocumentResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("solicitorDesignations/{documentId}")]
        public async Task<ActionResult<SolicitorDesignationInfoResponse>> GetSolicitorDesignationAsync([FromRoute] string documentId)
        {
            try
            {
                var bpnDocumentResponse = await _documentService.GetSolicitorDesignationAsync(documentId);

                return Ok(bpnDocumentResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("bpnResults/{documentId}")]
        public async Task<ActionResult<SolicitorDesignationInfoResponse>> GetBPNResultAsync([FromRoute] string documentId)
        {
            try
            {
                var bpnDocumentResponse = await _documentService.GetBPNResultAsync(documentId);

                return Ok(bpnDocumentResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("solicitorDossierRequests/{documentId}")]
        public async Task<ActionResult<SolicitorDossierRequestInfoResponse>> GetSolicitorDossierRequestAsync([FromRoute] string documentId)
        {
            try
            {
                var solicitorDossierRequestResponse = await _documentService.GetSolicitorDossierRequestAsync(documentId);

                return Ok(solicitorDossierRequestResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("solicitorDossierShipments/{documentId}")]
        public async Task<ActionResult<SolicitorDossierShipmentInfoResponse>> GetSolicitorDossierShipmentAsync([FromRoute] string documentId)
        {
            try
            {
                var solicitorDossierShipmentResponse = await _documentService.GetSolicitorDossierShipmentAsync(documentId);

                return Ok(solicitorDossierShipmentResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("signConclusion/{documentId}")]
        public async Task<ActionResult<SignConclusionInfoResponse>> GetSignConclusionAsync([FromRoute] string documentId)
        {
            try
            {
                var signConclusionResponse = await _documentService.GetSignConclusionAsync(documentId);

                return Ok(signConclusionResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("disciplinaryOpenness/{documentId}")]
        public async Task<ActionResult<DisciplinaryOpennessInfoResponse>> GetDisciplinaryOpennessAsync([FromRoute] string documentId)
        {
            try
            {
                var disciplinaryOpennessResponse = await _documentService.GetDisciplinaryOpennessAsync(documentId);

                return Ok(disciplinaryOpennessResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion

        #region POST private methods
        private async Task<InitialRequest> RegisterInitialRequestAsync(InitialRequestResponse document, User user)
        {
            var urls = await _mediaService.SaveFilesAsync(document.URLAnnex, _containerName);

            var initialRequestContent = _mapper.Map<InitialRequestContent>(document.Content);
            var initialRequest = new InitialRequest(initialRequestContent, "registrado", urls.ToList());
            initialRequest.AddProcess(new Process(user.Id, user.Id, "registrado", user.Rol));

            return await _documentService.InitialRequestRegisterAsync(initialRequest);
        }

        private async Task RegisterOutPutTrayAsync(Document document, User user, Dossier dossier)
        {
            var currentDocumentId = dossier.Documents[^2];

            var outputTrayDTO = new OutPutTrayDTO(dossier.Id, currentDocumentId.DocumentId, document.Id, user.Id);

            await _trayService.RegisterOutputTrayAsync(outputTrayDTO);
        }

        private async Task<ComplaintRequest> RegisterComplaintRequestAsync(ComplaintRequestResponse document, User user)
        {
            var urls = await _mediaService.SaveFilesAsync(document.URLAnnex, _containerName);

            var complaintRequestContent = _mapper.Map<ComplaintRequestContent>(document.Content);
            var complaintRequest = new ComplaintRequest(complaintRequestContent, "registrado", urls.ToList());
            complaintRequest.AddProcess(new Process(user.Id, user.Id, "registrado", user.Rol));

            return await _documentService.RegisterComplaintRequestAsync(complaintRequest);
        }

        private async Task<Dictum> RegisterDictumAsync(DictumResponse document, User user)
        {
            var urls = await _mediaService.SaveFilesAsync(document.URLAnnex, _containerName);

            var dictumContent = _mapper.Map<DictumContent>(document.Content);
            var dictum = new Dictum(dictumContent, "registrado", urls.ToList());
            dictum.AddProcess(new Process(user.Id, user.Id, "registrado", user.Rol));
            
            return await _documentService.RegisterDictumAsync(dictum);
        }

        private async Task<SolicitorDossierShipment> RegisterSolicitorDossierShipmentAsync(SolicitorDossierShipmentResponse document, User user)
        {
            var urls = await _mediaService.SaveFilesAsync(document.URLAnnex, _containerName);

            var solicitorDossierShipmentContent = _mapper.Map<SolicitorDossierShipmentContent>(document.Content);
            var solicitorDossierShipment = new SolicitorDossierShipment(solicitorDossierShipmentContent, "registrado", urls.ToList());
            solicitorDossierShipment.AddProcess(new Process(user.Id, user.Id, "registrado", user.Rol));

            return await _documentService.RegisterSolicitorDossierShipmentAsync(solicitorDossierShipment);
        }

        private async Task<Dossier> RegisterInitialDossierAsync(InitialRequestResponse document, InitialRequest initialRequest)
        {
            var client = new Shared.Entities.Client(document.ClientName, document.ClientLastName, document.DocumentNumber, document.DocumentType, document.ClientId);
            var dossier = new Dossier(client, "Solicitud", "registrado"); // In the past, the state was "solicitado"

            dossier.AddDocument(new DossierDocument(1, initialRequest.Id, "SolicitudInicial", DateTime.UtcNow.AddHours(-5).AddDays(10)));

            await _dossierService.CreateDossierAsync(dossier);

            return dossier;
        }

        private async Task<Dossier> UpdateDossierByDocumentAsync(DossierUpdateDTO dossierUpdateDTO)
        {
            var dossier = new Dossier(dossierUpdateDTO.DossierWrapper.Id!, dossierUpdateDTO.DossierType, dossierUpdateDTO.DossierState);

            dossier.AddDocument(dossierUpdateDTO.DossierDocument);

            return await _dossierService.UpdateDossierForInitialRequestAsync(dossier);
        }

        private static T DeserializeDocument<T>(object document)
        {
            string json = Json.JsonSerializer.Serialize(document);
            return Json.JsonSerializer.Deserialize<T>(json)!;
        }

        private async Task<User> GetUserAsync(string claimType)
        {
            var identifier = GetUserClaimValue(claimType);

            var user = await _userService.GetUserByIdAsync(identifier);

            return user;
        }

        private string GetUserClaimValue(string claimType)
        {   
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == claimType);

            if (userClaim is null) throw new Exception("Información no especificada en la cabecera de la petición");

            return userClaim.Value;

        }

        private async Task<DisciplinaryOpenness> RegisterDisciplinaryOpennessDocumentAsync(DisciplinaryOpennessResponse document, User user)
        {
            var urls = await _mediaService.SaveFilesAsync(document.URLAnnex, _containerName);

            var DisciplinaryOpennessContent = _mapper.Map<DisciplinaryOpennessContent>(document.Content);
            var DisciplinaryOpenness = new DisciplinaryOpenness(DisciplinaryOpennessContent, "registrado", urls.ToList());
            DisciplinaryOpenness.AddProcess(new Process(user.Id, user.Id, "registrado", user.Rol));

            return await _documentService.DisciplinaryOpennessRegisterAsync(DisciplinaryOpenness);
        }
        
        private async Task<SolicitorDossierRequest> RegisterSolicitorDossierRequestDocumentAsync(SolicitorDossierRequestResponse document, User user)
        {
            var urls = await _mediaService.SaveFilesAsync(document.URLAnnex, _containerName);

            var solicitorDossierRequestContent = _mapper.Map<SolicitorDossierRequestContent>(document.Content);
            var SolicitorDossierRequest = new SolicitorDossierRequest(solicitorDossierRequestContent, "registrado", urls.ToList());
            SolicitorDossierRequest.AddProcess(new Process(user.Id, user.Id, "registrado", user.Rol));

            return await _documentService.SolicitorDossierRequestRegisterAsync(SolicitorDossierRequest);
        }
        
        private async Task<Resolution> RegisterResolutionDocumentAsync(ResolutionResponse document, User user)
        {
            var urls = await _mediaService.SaveFilesAsync(document.URLAnnex, _containerName);

            var resolutionContent = _mapper.Map<ResolutionContent>(document.Content);
            var resolution = new Resolution(resolutionContent, "registrado", urls.ToList());
            resolution.AddProcess(new Process(user.Id, user.Id, "registrado", user.Rol));

            return await _documentService.ResolutionRegisterAsync(resolution);
        }
        

        private async Task<DocumentGenerationDTO> GetGeneratedDocumentAsync(GenerateDocumentRequest documentRequest, User user)
        {
            var signTask = _mediaService.SaveFileAsync(documentRequest.Sign, _generatedContainerName);
            var generatedUrlTask = _mediaService.SaveFileAsync(documentRequest.GeneratedURL, _generatedContainerName);

            await Task.WhenAll(signTask, generatedUrlTask);

            var documentGenerationDTO = _mapper.Map<DocumentGenerationDTO>(documentRequest);

            documentGenerationDTO.Sign = await signTask;
            documentGenerationDTO.GeneratedURL = await generatedUrlTask;
            documentGenerationDTO.UserId = user.Id;
            documentGenerationDTO.RoleId = user.Rol;

            return documentGenerationDTO;
        }

        #endregion

    }
}
