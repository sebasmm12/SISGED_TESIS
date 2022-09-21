using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SISGED.Server.Helpers;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.Appeal;
using SISGED.Shared.Models.Responses.Document.BPNDocument;
using SISGED.Shared.Models.Responses.Document.BPNRequest;
using SISGED.Shared.Models.Responses.Document.Dictum;
using SISGED.Shared.Models.Responses.Document.InitialRequest;
using SISGED.Shared.Models.Responses.Document.Resolution;
using SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.DossierDocument;

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

        public DocumentsController(IDocumentService documentService, IDossierService dossierService, ITrayService trayService, IPublicDeedsService publicDeedsService, IFileStorageService fileStorage, IAssistantService assistantService, IMapper mapper)
        {
            _documentService = documentService;
            _dossierService = dossierService;
            _trayService = trayService;
            _publicdeedsService = publicDeedsService;
            _fileService = fileStorage;
            _assistantService = assistantService;
            _mapper = mapper;
        }

        #region POST

        [HttpPost("documentoodn")]
        public async Task<ActionResult<SolicitorDesignationDocument>> SolicitorDesignationOfficeRegister(SolicitorDesignationDocumentRegister dossierWrapper)
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
        public async Task<ActionResult<BPNDocument>> OfficeBPNDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
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
        public async Task<ActionResult<BPNRequest>> BPNRequestDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
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

        [HttpPost("documentosd")]
        //public async Task<ActionResult<ExpedienteBandejaDTO>> RegistrarDocumentoSolicitudDenuncia(ExpedienteWrapper expedientewrapper)
        public async Task<ActionResult<ComplaintRequest>> ComplaintRequestDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                //conversion de Object a Tipo especifico
                ComplaintRequestResponse document = new ComplaintRequestResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                document = JsonConvert.DeserializeObject<ComplaintRequestResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in document.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "solicituddenuncia");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                //subida de archivo a repositorio y retorno de url
                string urlData = "";
                if (!string.IsNullOrWhiteSpace(document.Content.URLData))
                {
                    var solicitudBytes = Convert.FromBase64String(document.Content.URLData);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes, "pdf", "solicituddenuncia");
                    urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                }

                ComplaintRequest solicitudDenuncia = new ComplaintRequest();
                solicitudDenuncia = await _documentService.RegisterComplaintRequestAsync(dossierWrapper, url2, urlData);

                return Ok(solicitudDenuncia);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentosef")]
        //public async Task<ActionResult<ExpedienteBandejaDTO>> RegistrarDocumentoSEF(ExpedienteWrapper expedientewrapper)
        public async Task<ActionResult<SignExpeditionRequest>> RegisterSEFDocument(SolicitorDesignationDocumentRegister dossierWrapper)
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

        [HttpPost("registrarsolicitudinicial")]
        public async Task<ActionResult<DossierDocumentInitialRequestResponse>> InitialRequestDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                //Obtenemos los datos del expedientewrapper
                InitialRequestResponse doc = new InitialRequestResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                doc = JsonConvert.DeserializeObject<InitialRequestResponse>(json)!;

                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in doc.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "solicitudesiniciales");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }

                //Creacionde Obj y almacenamiento en la coleccion documento
                InitialRequestContent contenidoDTOInicial = new InitialRequestContent()
                {
                    Title = doc.Content.Title,
                    Description = doc.Content.Description,
                };

                InitialRequest soliInicial = new InitialRequest()
                {
                    Type = "SolicitudInicial",
                    Content = contenidoDTOInicial,
                    State = "pendiente",
                    AttachedUrls = url2,
                    ContentsHistory = new List<ContentVersion>(),
                    ProcessesHistory = new List<Process>()
                };

                soliInicial = await _documentService.InitialRequestRegisterAsync(soliInicial);

                //Creacionde del Obj. Expediente de Denuncia y registro en coleccion de expedientes
                Shared.Entities.Client cliente = new Shared.Entities.Client()
                {
                    Name = doc.ClientName,
                    DocumentType = doc.DocumentType,
                    DocumentNumber = doc.DocumentNumber
                };
                Dossier expediente = new Dossier();
                expediente.Type = "Solicitud";
                expediente.Client = cliente;
                expediente.StartDate = DateTime.UtcNow.AddHours(-5);
                expediente.EndDate = null;
                expediente.Documents = new List<DossierDocument>()
            {
                new DossierDocument(){
                    Index = 1,
                    DocumentId = soliInicial.Id,
                    Type  = "SolicitudInicial",
                    CreationDate = DateTime.UtcNow.AddHours(-5),
                    ExcessDate= DateTime.UtcNow.AddHours(-5).AddDays(10),
                    DelayDate = null
                }
            };
                expediente.Derivations = new List<Derivation>();
                expediente.State = "solicitado";
                await _dossierService.CreateDossierAsync(expediente);

                await _trayService.UserInputTrayInsertAsync(expediente.Id, soliInicial.Id, "josue");

                Assistant assistant = new Assistant();
                assistant.DossierId = expediente.Id;
                assistant.Steps = new AssistantStep();
                assistant.Steps.DossierName = "Solicitud";

                await _assistantService.CreateAsync(assistant);

                DossierDocumentInitialRequestResponse dossierDocIR = new DossierDocumentInitialRequestResponse();
                dossierDocIR.Dossier = expediente;
                dossierDocIR.InitialRequest = soliInicial;

                return Ok(dossierDocIR);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentocf")]
        public async Task<ActionResult<SignConclusion>> CFDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
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

        [HttpPost("documentoad")]
        public async Task<ActionResult<DisciplinaryOpenness>> DisciplinaryOpennessDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                //Deserealizacion de objeto de tipo AperturamientoDisciplinario
                DisciplinaryOpennessResponse DTO = new DisciplinaryOpennessResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<DisciplinaryOpennessResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "aperturamientodiciplinario");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                //Almacenando el pdf en el servidor de archivos y obtencion de la url
                string urlData = "";
                if (!string.IsNullOrWhiteSpace(DTO.Content.URL))
                {
                    var solicitudBytes = Convert.FromBase64String(DTO.Content.URL);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes, "pdf", "aperturamientodiciplinario");
                    urlData = await _fileService.SaveFileAsync(file) ?? string.Empty;
                }

                var disciplinaryOpenness = await _documentService.DisciplinaryOpennessRegisterAsync(DTO, urlData, url2, dossierWrapper.CurrentUserId, dossierWrapper.Id, dossierWrapper.InputDocument);
                return Ok(disciplinaryOpenness);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentod")]
        public async Task<ActionResult<Dictum>> DictumDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                DictumResponse DTO = new DictumResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<DictumResponse>(json)!;
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
                var dictum = await _documentService.DictumRegisterAsync(DTO, dossierWrapper, url2);
                return Ok(dictum);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentor")]
        public async Task<ActionResult<Resolution>> ResolutionDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                ResolutionResponse DTO = new ResolutionResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<ResolutionResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.UrlAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "resolucion");
                        urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                        url2.Add(urlData2);
                    }
                }
                //Almacenando el pdf en el servidor de archivos y obtencion de la url
                string urlData = "";
                if (!string.IsNullOrWhiteSpace(DTO.Content.Data))
                {
                    var solicitudBytes = Convert.FromBase64String(DTO.Content.Data);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes, "pdf", "resolucion");
                    urlData = await _fileService.SaveFileAsync(file) ?? string.Empty;
                }

                DossierResponse expedientePorConsultar = await _dossierService.GetDossierByIdAsync(dossierWrapper.Id);
                DossierDocument documentRequest = expedientePorConsultar.Documents.Find(x => x.Type == "SolicitudInicial")!;

                var resolution = await _documentService.ResolutionRegisterAsync(DTO, urlData, url2, dossierWrapper.CurrentUserId, dossierWrapper.Id, dossierWrapper.InputDocument, documentRequest.DocumentId);
                return Ok(resolution);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("documentoResultadoBPN")]
        public async Task<ActionResult<BPNResult>> BPNResultDocumentRegister(SolicitorDesignationDocumentRegister dossierWrapper)
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

        [HttpPost("documentoEntregaExpedienteNotario")]
        public async Task<ActionResult<SolicitorDossierShipment>> RegistrarDocumentoEntregaExpedienteNotario(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            //Deserealizacion de objeto de tipo C
            SolicitorDossierShipmentResponse DTO = new SolicitorDossierShipmentResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SolicitorDossierShipmentResponse>(json)!;

            List<string> url2 = new List<string>();
            string urlData2 = "";
            foreach (string u in DTO.Content.URLAnnex)
            {
                if (!string.IsNullOrWhiteSpace(u))
                {
                    var solicitudBytes2 = Convert.FromBase64String(u);
                    FileRegisterDTO file = new FileRegisterDTO(solicitudBytes2, "pdf", "entregaexpedientenotario");
                    urlData2 = await _fileService.SaveFileAsync(file) ?? string.Empty;
                    url2.Add(urlData2);
                }
            }
            return await _documentService.SolicitorDossierShipmentRegisterAsync(DTO, dossierWrapper, url2);
        }

        #endregion

        #region PUT
        [HttpPut("cambiarestado")]
        public async Task<ActionResult<Document>> ModifyState(Evaluation document, [FromQuery] string docId)
        {
            try
            {
                var state = await _documentService.ModifyStateAsync(document, docId);
                return Ok(state);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("generardocumento")]
        public async Task<ActionResult<Document>> GenerateDocument(GenerateDocumentRequest document)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(document.Sign))
                {
                    var solicitudBytes2 = Convert.FromBase64String(document.Sign);
                    document.Sign = await _fileService.saveDoc(solicitudBytes2, "png", "resultadobpn");
                }
                if (!string.IsNullOrWhiteSpace(document.GeneratedURL))
                {
                    var solicitudBytes2 = Convert.FromBase64String(document.GeneratedURL);
                    document.GeneratedURL = await _fileService.saveDoc(solicitudBytes2, "pdf", "resultadobpn");
                }

                var generated = await _documentService.GenerateDocumentAsync(document);
                return Ok(generated);
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
        public async Task<ActionResult<SolicitorDesignationDocument>> ModifyDocumentODN(SolicitorDesignationDocumentRegister dossierWrapper)
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
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "oficiodesignacionnotario");
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
        public async Task<ActionResult<Appeal>> AppealDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
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
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "apelaciones");
                        url2.Add(urlData2);
                    }
                }
                if (!string.IsNullOrWhiteSpace(DTO.Content.Data))
                {
                    var solicitudBytes = Convert.FromBase64String(DTO.Content.Data);
                    urlData = await _fileService.saveDoc(solicitudBytes, "pdf", "apelaciones");
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
        public async Task<ActionResult<DisciplinaryOpenness>> DisciplinaryOpennessDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                DisciplinaryOpennessResponse DTO = new DisciplinaryOpennessResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<DisciplinaryOpennessResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "aperturamientodiciplinario");
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
        public async Task<ActionResult<SignConclusion>> SignConclusionDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
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
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "dictamen");
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
        public async Task<ActionResult<Dictum>> DictumDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                DictumResponse DTO = new DictumResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<DictumResponse>(json)!;
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.URLAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "dictamen");
                        url2.Add(urlData2);
                    }
                }

                return await _documentService.UpdateDictumDocumentAsync(dossierWrapper, url2);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("actualizarDocumentoOficioBPN")]
        public async Task<ActionResult<BPNDocument>> BPNOfficeDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
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
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "oficiobpn");
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
        public async Task<ActionResult<Resolution>> ResolutionDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                ResolutionResponse DTO = new ResolutionResponse();
                var json = JsonConvert.SerializeObject(dossierWrapper.Document);
                DTO = JsonConvert.DeserializeObject<ResolutionResponse>(json)!;
                string urlData = "";
                List<string> url2 = new List<string>();
                string urlData2 = "";
                foreach (string u in DTO.Content.UrlAnnex)
                {
                    if (!string.IsNullOrWhiteSpace(u))
                    {
                        var solicitudBytes2 = Convert.FromBase64String(u);
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "resolucion");
                        url2.Add(urlData2);
                    }
                }
                if (!string.IsNullOrWhiteSpace(DTO.Content.Data))
                {
                    var solicitudBytes = Convert.FromBase64String(DTO.Content.Data);
                    urlData = await _fileService.saveDoc(solicitudBytes, "pdf", "resolucion");
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
        public async Task<ActionResult> SENDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
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
        public async Task<ActionResult<BPNResult>> BPNResultDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
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
                        urlData2 = await _fileService.saveDoc(solicitudBytes2, "pdf", "resultadobpn");
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
        public async Task<ActionResult> InitialRequestDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                await _documentService.UpdateInitialRequestDocumentAsync(dossierWrapper);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("actualizarDocumentoEEN")]
        public async Task<ActionResult> EENDocumentModify(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                await _documentService.UpdateEENDocumentAsync(dossierWrapper);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("estadosolicitud")]
        public async Task<ActionResult> InitialRequestStateModify(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            try
            {
                await _documentService.UpdateInitialRequestStateAsync(dossierWrapper);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

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

        #endregion

    }
}
