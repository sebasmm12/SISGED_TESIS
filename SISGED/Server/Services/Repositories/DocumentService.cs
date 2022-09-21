using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.BPNDocument;
using SISGED.Shared.Models.Responses.Document.BPNResult;
using SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness;
using SISGED.Shared.Models.Responses.Document.SignConclusion;
using SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentService : IDocumentService
    {
        private readonly IMongoCollection<Document> _documentsCollection;
        private readonly IMongoCollection<Tray> _trayCollection;
        private readonly IDossierService _dossierService;
        public string CollectionName => "documentos";
        public string TrayCollectionName => "bandejas";

        public DocumentService(IMongoDatabase mongoDatabase, IDossierService dossierService)
        {
            _documentsCollection = mongoDatabase.GetCollection<Document>(CollectionName);
            _trayCollection = mongoDatabase.GetCollection<Tray>(TrayCollectionName);

            _dossierService = dossierService;
        }

        public async Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber)
        {
            return await _dossierService.GetUserRequestDocumentsAsync(documentNumber);
        }

        public async Task<IEnumerable<UserRequestWithPublicDeedResponse>> GetUserRequestsWithPublicDeedAsync(string documentNumber)
        {
            return await _dossierService.GetUserRequestsWithPublicDeedAsync(documentNumber);
        }

        public async Task<BPNRequest> GetBPNRequestDocumentAsync(string documentId)
        {
            var bpnRequestDocument = await _documentsCollection.OfType<BPNRequest>().Find(document => document.Id == documentId).FirstAsync();

            if (bpnRequestDocument is null) throw new Exception($"No se pudo obtener la solicitud de búsqueda de protocolo notarial con el identificador {documentId}");

            return bpnRequestDocument;

        }

        public async Task<Dictum> GetDictumDocumentAsync(string documentId)
        {
            var dictumDocument = await _documentsCollection.OfType<Dictum>().Find(document => document.Id == documentId).FirstAsync();

            if (dictumDocument is null) throw new Exception($"No se pudo obtener el dictamen con el identificador {documentId}");

            return dictumDocument;
        }

        public async Task<Resolution> GetResolutionDocumentAsync(string documentId)
        {
            var resolutionDocument = await _documentsCollection.OfType<Resolution>().Find(document => document.Id == documentId).FirstAsync();

            if (resolutionDocument is null) throw new Exception($"No se pudo obtener la resolución con el identificador {documentId}");

            return resolutionDocument;
        }

        public async Task<Appeal> GetAppealDocumentAsync(string documentId)
        {
            var appealDocument = await _documentsCollection.OfType<Appeal>().Find(document => document.Id == documentId).FirstAsync();

            if (appealDocument is null) throw new Exception($"No se pudo obtener el recurso de apelación con el identificador {documentId}");

            return appealDocument;
        }

        public async Task<DocumentResponse> GetDocumentAsync(string documentId)
        {
            var document = await _documentsCollection
                                        .Find(document => document.Id == documentId)
                                        .Project(Builders<Document>.Projection.As<DocumentResponse>()).FirstAsync();

            if (document is null) throw new Exception($"No se pudo obtener el documento con identificador {documentId}");

            return document;
        }

        public async Task<InitialRequest> GetInitialRequestDocumentAsync(string documentId)
        {
            var initialRequestDocument = await _documentsCollection.OfType<InitialRequest>().Find(document => document.Id == documentId).FirstAsync();

            if (initialRequestDocument is null) throw new Exception($"No se pudo obtener la solicitud inicial con identificador {documentId}");

            return initialRequestDocument;
        }

        public async Task<BPNDocumentInfoResponse> GetBPNDocumentAsync(string documentId)
        {
            var bpnDocument = await _documentsCollection.Aggregate<BPNDocumentInfoResponse>(GetBPNDocumentPipeline(documentId)).FirstAsync();

            if (bpnDocument is null) throw new Exception($"No se pudo obtener el oficio de protocolo notarial con identificador {documentId}");

            return bpnDocument;
        }

        public async Task<SolicitorDesignationInfoResponse> GetSolicitorDesignationAsync(string documentId)
        {
            var solicitorDesignationDocument = await _documentsCollection.Aggregate<SolicitorDesignationInfoResponse>(GetSolicitorDesignationPipeline(documentId)).FirstAsync();

            if (solicitorDesignationDocument is null) throw new Exception($"No se pudo obtener el oficio de designación de notario con identificador {documentId}");

            return solicitorDesignationDocument;
        }

        public async Task<BPNResultInfoResponse> GetBPNResultAsync(string documentId)
        {
            var bpnResulDocument = await _documentsCollection.Aggregate<BPNResultInfoResponse>(GetBPNResultPipeline(documentId)).FirstAsync();

            if (bpnResulDocument is null) throw new Exception($"No se pudo obtener el resultado de la búsqueda de protocolo notario con identificador {documentId}");

            return bpnResulDocument;
        }

        public async Task<SolicitorDossierRequestInfoResponse> GetSolicitorDossierRequestAsync(string documentId)
        {
            var solicitorDossierRequestDocument = await _documentsCollection.Aggregate<SolicitorDossierRequestInfoResponse>(GetSolicitorDossierRequestPipeline(documentId)).FirstAsync();

            if (solicitorDossierRequestDocument is null) throw new Exception($"No se pudo obtener la solicitud de expediente de notario con identificador {documentId}");

            return solicitorDossierRequestDocument;
        }

        public async Task<SolicitorDossierShipmentInfoResponse> GetSolicitorDossierShipmentAsync(string documentId)
        {
            var solicitorDossierShipmentDocument = await _documentsCollection.Aggregate<SolicitorDossierShipmentInfoResponse>(GetSolicitorDossierShipmentPipeline(documentId)).FirstAsync();

            if (solicitorDossierShipmentDocument is null) throw new Exception($"No se pudo obtener el envío de expediente de notario con identificador {documentId}");

            return solicitorDossierShipmentDocument;
        }

        public async Task<SignConclusionInfoResponse> GetSignConclusionAsync(string documentId)
        {
            var signConclusionDocument = await _documentsCollection.Aggregate<SignConclusionInfoResponse>(GetSignConclusionPipeline(documentId)).FirstAsync();

            if (signConclusionDocument is null) throw new Exception($"No se pudo obtener la conclusión de firma con identificador {documentId}");

            return signConclusionDocument;
        }

        public async Task<DisciplinaryOpennessInfoResponse> GetDisciplinaryOpennessAsync(string documentId)
        {
            var disciplinaryOpennessDocument = await _documentsCollection.Aggregate<DisciplinaryOpennessInfoResponse>(GetDisciplinaryOpennessPipeline(documentId)).FirstAsync();

            if (disciplinaryOpennessDocument is null) throw new Exception($"No se pudo obtener el aperturamiento disciplinario con identificador {documentId}");

            return disciplinaryOpennessDocument;
        }

        public async Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAndAreaAsync(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery)
        {
            var documentsByMonthAndArea = await _documentsCollection.Aggregate<DocumentsByMonthAndAreaResponse>(GetDocumentsByMonthAndAreaPipeline(documentsByMonthAndAreaQuery)).ToListAsync();

            if (documentsByMonthAndArea is null) throw new Exception($"No se pudo obtener los documentos en el {documentsByMonthAndAreaQuery.Month} mes y en el área {documentsByMonthAndAreaQuery.Area}");

            return documentsByMonthAndArea;
        }

        public async Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery)
        {
            var documentsByMonth = await _documentsCollection.Aggregate<DocumentsByMonthAndAreaResponse>(GetDocumentsByMonthPipeline(documentsByMonthQuery)).ToListAsync();

            if (documentsByMonth is null) throw new Exception($"No se pudo obtener los documentos en el {documentsByMonthQuery.Month} mes");

            return documentsByMonth;
        }

        public async Task<IEnumerable<ExpiredDocumentsResponse>> GetExpiredDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery)
        {
            var expiredDocumentsByMonth = await _documentsCollection.Aggregate<ExpiredDocumentsResponse>(GetExpiredDocumentsByMonthPipeline(documentsByMonthQuery)).ToListAsync();

            if (expiredDocumentsByMonth is null) throw new Exception($"No se pudo obtener los documentos expirados en el {documentsByMonthQuery.Month} mes");

            return expiredDocumentsByMonth;
        }

        public async Task<IEnumerable<DocumentByStateResponse>> GetDocumentsByStateAsync(DocumentsByStateQuery documentsByStateQuery)
        {
            var documentsByState = await _documentsCollection.Aggregate<DocumentByStateResponse>(GetDocumentsByStatePipeline(documentsByStateQuery)).ToListAsync();

            if (documentsByState is null) throw new Exception($"No se pudo obtener los documentos por estado en el {documentsByStateQuery.Month} mes");

            return documentsByState;
        }

        public async Task UpdateDocumentProcessAsync(Process proccess, string documentId)
        {
            var updateDocumentProccess = Builders<Document>.Update.Push(document => document.ProcessesHistory, proccess);

            var updatedDocument = await _documentsCollection.UpdateOneAsync(document => document.Id == documentId, updateDocumentProccess);

            if (updatedDocument is null) throw new Exception($"No se pudo actualizar el historial del proceso del documento con identificador {documentId}");
        }

        public async Task<SolicitorDesignationDocument> SolicitorDesignationOfficeRegisterAsync(SolicitorDesignationDocumentRegister dossierwrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo OficioDesignacionNotarioDTO
            SolicitorDesignationDocumentResponse oficioDesignacionNotarioDTO = new SolicitorDesignationDocumentResponse();
            var json = JsonConvert.SerializeObject(dossierwrapper.Document);
            oficioDesignacionNotarioDTO = JsonConvert.DeserializeObject<SolicitorDesignationDocumentResponse>(json)!;

            //Creacion de Obj OficioDesignacionNotario y registro en coleccion de documentos 
            SolicitorDesignationDocumentContent contenidoODN = new SolicitorDesignationDocumentContent()
            {
                Code = "",
                Title = oficioDesignacionNotarioDTO.content.Title,
                Description = oficioDesignacionNotarioDTO.content.Description,
                RealizationDate = DateTime.UtcNow.AddHours(-5),
                SolicitorAddress = oficioDesignacionNotarioDTO.content.SolicitorOfficeLocation,
                UserId = oficioDesignacionNotarioDTO.content.UserId,
                SolicitorId = oficioDesignacionNotarioDTO.content.SolicitorId.Id,
                Sign = ""
            };
            SolicitorDesignationDocument documentoODN = new SolicitorDesignationDocument()
            {
                Type = "OficioDesignacionNotario",
                Content = contenidoODN,
                Evaluation = new Evaluation()
                {
                    Result = "pendiente",
                    Evaluations = new List<IndividualEvaluation>()
                },
                State = "creado",
                AttachedUrls = url2,
                ContentsHistory = new List<ContentVersion>(),
                ProcessesHistory = new List<Process>()
            };
            await _documentsCollection.InsertOneAsync(documentoODN);

            //Actualizacion del expediente
            DossierDocument dossierDocument = new DossierDocument();
            dossierDocument.Index = 2;
            dossierDocument.DocumentId = documentoODN.Id;
            dossierDocument.Type = "OficioDesignacionNotario";
            dossierDocument.CreationDate = DateTime.UtcNow.AddHours(-5);
            dossierDocument.DelayDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            dossierDocument.DelayDate = null;
            UpdateDefinition<Dossier> updateExpediente = Builders<Dossier>.Update.Push("documentos", dossierDocument);
            Dossier dossier = await _dossierCollection.FindOneAndUpdateAsync(x => x.Id == dossierwrapper.Id, updateExpediente);
            // TODO: IMPLEMENT A NEW SERVICE IN THE DOSSIER SERVICE

            //actualizacion bandeja salida del usuario
            /*BandejaDocumento bandejaSalidaDocumento = new BandejaDocumento();
            bandejaSalidaDocumento.idexpediente = expediente.id;
            bandejaSalidaDocumento.iddocumento = documentoExpediente.iddocumento;
            UpdateDefinition<Bandeja> updateBandejaSalida = Builders<Bandeja>.Update.Push("bandejasalida", bandejaSalidaDocumento);
            _bandejas.UpdateOne(band => band.usuario == expedienteWrapper.idusuarioactual, updateBandejaSalida);*/

            //actualizacion bandeja de entrada del usuario
            /*UpdateDefinition<Bandeja> updateBandejaEntrada =
                Builders<Bandeja>.Update.PullFilter("bandejaentrada",
                  Builders<BandejaDocumento>.Filter.Eq("iddocumento", expedienteWrapper.documentoentrada));
            _bandejas.UpdateOne(band => band.usuario == expedienteWrapper.idusuarioactual, updateBandejaEntrada);*/

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", dossierwrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);
            return documentoODN;
        }

        public async Task<BPNDocument> RegisterBPNOfficeAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2, string url)
        {
            //Obtenemos los datos del expedientewrapper
            BPNOfficeResponse oficioBPNDTO = new BPNOfficeResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            oficioBPNDTO = JsonConvert.DeserializeObject<BPNOfficeResponse>(json)!;

            List<String> otor = new List<string>();
            foreach (BPNGranter ot in oficioBPNDTO.Content.Granters)
            {
                otor.Add(ot.Name);
            }

            //Insertando el oficio normal
            BPNDocumentContent contenidoSolicitudBPN = new BPNDocumentContent()
            {
                Code = "",
                Title = oficioBPNDTO.Content.Title,
                Description = oficioBPNDTO.Content.Description,
                ClientId = oficioBPNDTO.Content.ClientId.Id,
                DocumentAddress = oficioBPNDTO.Content.DocumentAddress,
                SolicitorId = oficioBPNDTO.Content.SolicitorId.Id,
                JuridicalAct = oficioBPNDTO.Content.LegalAct,
                ProtocolType = oficioBPNDTO.Content.ProtocolType,
                Grantors = otor,
                RealizationDate = DateTime.UtcNow.AddHours(-5),
                Url = url,
                Sign = ""
            };
            BPNDocument documentoBPN = new BPNDocument()
            {
                Type = "OficioBPN",
                Content = contenidoSolicitudBPN,
                //estado = "pendiente",
                Evaluation = new Evaluation()
                {
                    Result = "pendiente",
                    Evaluations = new List<IndividualEvaluation>(),
                },
                State = "Creado",
                ContentsHistory = new List<ContentVersion>(),
                AttachedUrls = url2,
                ProcessesHistory = new List<Process>()
            };
            await _documentsCollection.InsertOneAsync(documentoBPN);

            //Actualizamos el expediente y agregamos el documento a sus documentos contenidos
            DossierDocument dossierDocument = new DossierDocument();
            dossierDocument.Index = 2;
            dossierDocument.DocumentId = documentoBPN.Id;
            dossierDocument.Type = "OficioBPN";
            dossierDocument.CreationDate = DateTime.UtcNow.AddHours(-5);
            dossierDocument.ExcessDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            dossierDocument.DelayDate = null;

            UpdateDefinition<Dossier> updateExpediente = Builders<Dossier>.Update.Push("documentos", dossierDocument);
            Dossier expediente = await _dossierCollection.FindOneAndUpdateAsync(x => x.Id == dossierWrapper.Id, updateExpediente);
            // TODO: IMPLEMENT A NEW SERVICE IN THE DOSSIER SERVICE

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);
            return documentoBPN;
        }

        public async Task<BPNRequest> RegisterBPNRquestAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Obtenemos los datos del expedientewrapper
            BPNRequestResponse document = new BPNRequestResponse();
            BPNRequestResponseContent listaotor = new BPNRequestResponseContent();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            document = JsonConvert.DeserializeObject<BPNRequestResponse>(json)!;

            //Solo para registrar nombre de otorgantes
            List<String> grantorslist = new List<string>();
            foreach (GrantorList obs in document.Content.GrantorLists)
            {
                grantorslist.Add(obs.Name);
            }

            //Creacionde Obj ContenidoSolicitudBPN y almacenamiento en la coleccion documento
            BPNRequestContent content = new BPNRequestContent()
            {
                Code = "",
                ClientId = document.Content.ClientId.Id,
                DocumentAddress = document.Content.SolicitorAddress,
                SolicitorId = document.Content.SolicitorId.Id,
                JuridicalAct = document.Content.LegalAct,
                ProtocolType = document.Content.ProtocolType,
                Grantors = grantorslist,
                RealizationDate = DateTime.UtcNow.AddHours(-5),
                Sign = ""
            };

            BPNRequest solicitudBPN = new BPNRequest()
            {
                Type = "SolicitudBPN",
                Content = content,
                State = "pendiente",
                AttachedUrls = url2,
                ContentsHistory = new List<ContentVersion>(),
                ProcessesHistory = new List<Process>()
            };

            Dossier dossier = new Dossier();
            dossier.Id = dossierWrapper.Id;
            dossier.Type = "Busqueda Protocolo Notarial";


            await _documentsCollection.InsertOneAsync(solicitudBPN);

            dossier.Documents = new List<DossierDocument>()
            {
                new DossierDocument(){
                    Index = 2,
                    DocumentId = solicitudBPN.Id,
                    Type="SolicitudBPN",
                    CreationDate = solicitudBPN.Content.RealizationDate,
                    ExcessDate=solicitudBPN.Content.RealizationDate.AddDays(10),
                    DelayDate = null
                }
            };

            await _dossierService.UpdateDossierForInitialRequestAsync(dossier);

            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            return solicitudBPN;
        }

        public async Task<ComplaintRequest> RegisterComplaintRequestAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2, string urlData)
        {
            //conversion de Object a Tipo especifico
            ComplaintRequestResponse documento = new ComplaintRequestResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            documento = JsonConvert.DeserializeObject<ComplaintRequestResponse>(json)!;

            //Creacionde Obj ContenidoSolicitudDenuncia y almacenamiento en la coleccion documento
            ComplaintRequestContent content = new ComplaintRequestContent()
            {
                Code = documento.Content.Code,
                Title = documento.Content.Title,
                Description = documento.Content.Description,
                ClientName = documento.ClientName,
                DeliveryDate = DateTime.UtcNow.AddHours(-5),
                Url = urlData
            };

            ComplaintRequest solicitudDenuncia = new ComplaintRequest()
            {
                Type = "SolicitudDenuncia",
                Content = content,
                State = "pendiente",
                ContentsHistory = new List<ContentVersion>(),
                AttachedUrls = url2,
                ProcessesHistory = new List<Process>(),
            };

            Dossier expediente = new Dossier();
            expediente.Id = dossierWrapper.Id;
            expediente.Type = "Denuncia";

            expediente.Derivations = new List<Derivation>();
            expediente.State = "solicitado";

            await _documentsCollection.InsertOneAsync(solicitudDenuncia);

            expediente.Documents = new List<DossierDocument>()
            {
                new DossierDocument(){
                    Index = 1,
                    DocumentId = solicitudDenuncia.Id,
                    Type="SolicitudDenuncia",
                    CreationDate = solicitudDenuncia.Content.DeliveryDate,
                    ExcessDate=solicitudDenuncia.Content.DeliveryDate.AddDays(10),
                    DelayDate = null
                }
            };

            await _dossierService.UpdateDossierForInitialRequestAsync(expediente);

            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            _documentsCollection.UpdateOne(filter, update);

            return solicitudDenuncia;
        }

        public async Task<SignExpeditionRequest> RegisterSignExpeditionRequestAsync(SolicitorDesignationDocumentRegister dossierWrapepr, List<string> url2, string urlData)
        {
            //Conversion de Obj a tipo SolicitudExpedicionFirmaDTO
            SignExpeditionRegisterResponse solicitudExpedicionFirmasDTO = new SignExpeditionRegisterResponse();
            var json = JsonConvert.SerializeObject(dossierWrapepr.Document);
            solicitudExpedicionFirmasDTO = JsonConvert.DeserializeObject<SignExpeditionRegisterResponse>(json)!;

            //Registro de objeto ContenidoSolicitudExpedicionFirma y registro en coleccion documentos
            SignExpeditionRequestContent contentSEF = new SignExpeditionRequestContent()
            {
                Title = solicitudExpedicionFirmasDTO.Content.Title,
                Description = solicitudExpedicionFirmasDTO.Content.Description,
                RealizationDate = DateTime.UtcNow.AddHours(-5),
                Client = solicitudExpedicionFirmasDTO.ClientName,
                Code = solicitudExpedicionFirmasDTO.Content.Code,
                Url = urlData
            };
            SignExpeditionRequest documentSEF = new SignExpeditionRequest()
            {
                Type = "SolicitudExpedicionFirma",
                Content = contentSEF,
                State = "pendiente",
                ContentsHistory = new List<ContentVersion>(),
                AttachedUrls = url2,
                ProcessesHistory = new List<Process>()
            };

            //Creacion del objeto Expediente y registro en la coleccion Expedientes
            Dossier expediente = new Dossier();
            expediente.Id = dossierWrapepr.Id;
            expediente.Type = "Expedicion de Firmas";

            expediente.Derivations = new List<Derivation>();
            expediente.State = "solicitado";

            await _documentsCollection.InsertOneAsync(documentSEF);

            expediente.Documents = new List<DossierDocument>()
            {
                new DossierDocument(){
                    Index = 1,
                    DocumentId = documentSEF.Id,
                    Type="SolicitudExpedicionFirma",
                    CreationDate = documentSEF.Content.RealizationDate,
                    ExcessDate = documentSEF.Content.RealizationDate.AddDays(10),
                    DelayDate = null
                }
            };

            await _dossierService.UpdateDossierForInitialRequestAsync(expediente);

            var filter = Builders<Document>.Filter.Eq("id", dossierWrapepr.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            return documentSEF;
        }

        public async Task<InitialRequest> InitialRequestRegisterAsync(InitialRequest documentIR)
        {
            await _documentsCollection.InsertOneAsync(documentIR);
            return documentIR;
        }

        public async Task<SignConclusion> singConclusionERegisterAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2, string documentId)
        {
            //Obtenemos los datos del expedientewrapper
            SignConclusionResponse DTO = new SignConclusionResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SignConclusionResponse>(json)!;

            //Insertando la conclusion normal
            SignConclusionContent contentCF = new SignConclusionContent()
            {
                Code = "",
                PublicDeedId = DTO.Content.PublicDeedId.Id,
                SolicitorId = DTO.Content.SolicitorId.Id,
                ClientId = DTO.Content.ClientId.Id,
                TotalSheets = DTO.Content.PageQuantity,
                Price = (DTO.Content.PageQuantity * 30),
                Sign = ""
            };

            SignConclusion documentoDF = new SignConclusion()
            {
                Type = "ConclusionFirma",
                Content = contentCF,
                State = "pendiente",
                ContentsHistory = new List<ContentVersion>(),
                AttachedUrls = url2,
                ProcessesHistory = new List<Process>()
            };
            await _documentsCollection.InsertOneAsync(documentoDF);

            //Actualizamos el expediente y agregamos el documento a sus documentos contenidos
            DossierDocument documentoExpediente = new DossierDocument();
            documentoExpediente.Index = 8;
            documentoExpediente.DocumentId = documentoDF.Id;
            documentoExpediente.Type = "ConclusionFirma";
            documentoExpediente.CreationDate = DateTime.UtcNow.AddHours(-5);
            documentoExpediente.ExcessDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            documentoExpediente.DelayDate = null;

            UpdateDefinition<Dossier> updateExpediente = Builders<Dossier>.Update.Push("documentos", documentoExpediente);
            Dossier expediente = await _dossierCollection.FindOneAndUpdateAsync(x => x.Id == dossierWrapper.Id, updateExpediente);
            // TODO: IMPLEMENT A NEW SERVICE IN THE DOSSIER SERVICE

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            //Actualizar el documento de solicitud a emitido, obtener el id de la solicitudBPN
            if (!string.IsNullOrEmpty(documentId))
            {
                var filterS = Builders<Document>.Filter.Eq("id", documentId);
                var updateS = Builders<Document>.Update
                    .Set("estado", "emitido");
                await _documentsCollection.UpdateOneAsync(filterS, updateS);
            }
            return documentoDF;
        }

        public async Task<DisciplinaryOpenness> DisciplinaryOpennessRegisterAsync(DisciplinaryOpennessResponse DTO,
            string urlData, List<string> url2, string userId, string dossierID, string inputDocId)
        {
            //Creacionde le objeto de AperturamientoDisciplinario y registro en la coleccion documentos
            DisciplinaryOpennessContent contenidoAD = new DisciplinaryOpennessContent()
            {
                Code = "",
                SolicitorId = DTO.Content.SolicitorId.Id,
                FiscalId = DTO.Content.ProsecutorId,
                ComplainantName = DTO.Content.Complainant,
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                AudienceStartDate = DTO.Content.AudienceStartDate,
                AudienceEndDate = DTO.Content.AudienceEndDate,
                Participants = DTO.Content.Participants.Select(x => x.Name).ToList(),
                AudiencePlace = DTO.Content.AudienceLocation,
                ImputedFacts = DTO.Content.ChargedDeeds.Select(x => x.Description).ToList(),
                Url = urlData,
                Sign = ""
            };
            DisciplinaryOpenness disciplinaryOpenness = new DisciplinaryOpenness()
            {
                Type = "AperturamientoDisciplinario",
                Content = contenidoAD,
                ContentsHistory = new List<ContentVersion>(),
                ProcessesHistory = new List<Process>(),
                AttachedUrls = url2,
                State = "creado"
            };
            await _documentsCollection.InsertOneAsync(disciplinaryOpenness);

            //Actualizacion del expediente
            Dossier dossier = new Dossier();
            DossierDocument dossierDocument = new DossierDocument();
            dossierDocument.Index = 8;
            dossierDocument.DocumentId = disciplinaryOpenness.Id;
            dossierDocument.Type = "AperturamientoDisciplinario";
            dossierDocument.CreationDate = DateTime.UtcNow.AddHours(-5);
            dossierDocument.ExcessDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            dossierDocument.DelayDate = null;
            dossier = await UpdateDossierAsync(dossierDocument, dossierID);

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", inputDocId);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);
            return disciplinaryOpenness;
        }

        public async Task<Dossier> UpdateDossierAsync(DossierDocument dossierDocument, string dossierId)
        {
            UpdateDefinition<Dossier> update = Builders<Dossier>.Update.Push("documentos", dossierDocument);
            Dossier dossier = await _dossierCollection.FindOneAndUpdateAsync(x => x.Id == dossierId, update);
            // TODO: IMPLEMENT A NEW SERVICE IN THE DOSSIER SERVICE
            return dossier;
        }

        public async Task<Dictum> DictumRegisterAsync(DictumResponse DTO, SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Obtenemos los datos del expedientewrapper

            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<DictumResponse>(json)!;

            //creacion del Objeto de tipo Dictamen y el registro en la coleccion Dictamen
            DictumContent content = new DictumContent()
            {
                Code = "",
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                ComplainantName = DTO.Content.ComplainantName,
                Conclusion = DTO.Content.Conclusion,
                Observations = DTO.Content.Observations.Select(x => x.Description).ToList(),
                Recommendations = DTO.Content.Recomendations.Select(x => x.Description).ToList(),
                Sign = ""
                //fecha           
            };
            Dictum dictum = new Dictum()
            {
                Type = "Dictamen",
                Content = content,
                ContentsHistory = new List<ContentVersion>(),
                ProcessesHistory = new List<Process>(),
                State = "creado",
                AttachedUrls = url2
            };
            await _documentsCollection.InsertOneAsync(dictum);

            //actualizacion de expediente
            Dossier dossier = new Dossier();
            DossierDocument dossierDocument = new DossierDocument();
            dossierDocument.Index = 8;
            dossierDocument.DocumentId = dictum.Id;
            dossierDocument.Type = "Dictamen";
            dossierDocument.CreationDate = DateTime.UtcNow.AddHours(-5);
            dossierDocument.ExcessDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            dossierDocument.DelayDate = null;
            dossier = await UpdateDossierAsync(dossierDocument, dossierWrapper.Id);

            //actualizando Bandeja salida
            /*BandejaDocumento bandejaDocumento = new BandejaDocumento();
            bandejaDocumento.idexpediente = expediente.id;
            bandejaDocumento.iddocumento = documentoExpediente.iddocumento;
            UpdateDefinition<Bandeja> updateBandeja = Builders<Bandeja>.Update.Push("bandejasalida", bandejaDocumento);
            _bandejas.UpdateOne(band => band.usuario == expedientewrapper.idusuarioactual, updateBandeja);*/

            //actualizando Bandeja Entrada
            /*UpdateDefinition<Bandeja> updateBandejaEntrada =
               Builders<Bandeja>.Update.PullFilter("bandejaentrada",
                 Builders<BandejaDocumento>.Filter.Eq("iddocumento", expedientewrapper.documentoentrada));
            _bandejas.UpdateOne(band => band.usuario == expedientewrapper.idusuarioactual, updateBandejaEntrada);*/

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);
            return dictum;
        }

        public async Task<Resolution> ResolutionRegisterAsync(ResolutionResponse DTO,
            string urlData, List<string> url2, string userId, string dossierId, string inputDocId, string documentRequestId)
        {
            //Creacionde le objeto de AperturamientoDisciplinario y registro en la coleccion documentos
            ResolutionContent contenidoResolucion = new ResolutionContent()
            {
                Code = "",
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                AudienceStartDate = DTO.Content.AudienceStartDate,
                AudienceEndDate = DTO.Content.AudienceEndDate,
                Participants = DTO.Content.Participants.Select(x => x.Name).ToList(),
                Sanction = DTO.Content.Penalty,
                Url = urlData,
                Sign = ""
            };
            Resolution resolucion = new Resolution()
            {
                Type = "Resolucion",
                Content = contenidoResolucion,
                ContentsHistory = new List<ContentVersion>(),
                ProcessesHistory = new List<Process>(),
                AttachedUrls = url2,
                Evaluation = new Evaluation()
                {
                    Result = "pendiente",
                    Evaluations = new List<IndividualEvaluation>()
                },
                State = "creado"
            };
            await _documentsCollection.InsertOneAsync(resolucion);

            //Actualizacion del expediente
            Dossier dossier = new Dossier();
            DossierDocument dossierDocument = new DossierDocument();
            dossierDocument.Index = 8;
            dossierDocument.DocumentId = resolucion.Id;
            dossierDocument.Type = "Resolucion";
            dossierDocument.CreationDate = DateTime.UtcNow.AddHours(-5);
            dossierDocument.ExcessDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            dossierDocument.DelayDate = null;
            dossier = await UpdateDossierAsync(dossierDocument, dossierId);

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", inputDocId);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            //Actualizar el documento de solicitud inicial a finalizado
            if (!String.IsNullOrEmpty(documentRequestId))
            {
                var filterS = Builders<Document>.Filter.Eq("id", documentRequestId);
                var updateS = Builders<Document>.Update
                       .Set("estado", "finalizado");

                await _documentsCollection.UpdateOneAsync(filterS, updateS);
            }
            return resolucion;
        }

        public async Task<BPNResult> BPNResultRegisterAsync(BPNResultResponse DTO, List<string> url2,
            string UserId, string dossierId, string inputDocId, string documentRequestId)
        {
            //Creacionde le objeto y registro en la coleccion documentos
            BPNResultContent content = new BPNResultContent()
            {
                Code = "",
                TotalSheets = DTO.Content.PageQuantity,
                Cost = DTO.Content.Cost,
                PublicDeedId = DTO.Content.PublicDeedId.Id,
                Status = "pendiente",
                Sign = ""
            };
            BPNResult bPNResult = new BPNResult()
            {
                Type = "ResultadoBPN",
                Content = content,
                ContentsHistory = new List<ContentVersion>(),
                ProcessesHistory = new List<Process>(),
                AttachedUrls = url2,
                State = "creado"
            };
            await _documentsCollection.InsertOneAsync(bPNResult);

            //Actualizacion del expediente
            Dossier expediente = new Dossier();
            DossierDocument dossierDocument = new DossierDocument();
            dossierDocument.Index = 8;
            dossierDocument.DocumentId = bPNResult.Id;
            dossierDocument.Type = "ResultadoBPN";
            dossierDocument.CreationDate = DateTime.UtcNow.AddHours(-5);
            dossierDocument.ExcessDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            dossierDocument.DelayDate = null;
            expediente = await UpdateDossierAsync(dossierDocument, dossierId);

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", inputDocId);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            //Actualizar el documento de solicitud a emitido, obtener el id de la solicitudBPN
            if (!string.IsNullOrEmpty(documentRequestId))
            {
                var filterS = Builders<Document>.Filter.Eq("id", documentRequestId);
                var updateS = Builders<Document>.Update
                    .Set("estado", "emitido");
                await _documentsCollection.UpdateOneAsync(filterS, updateS);
            }

            return bPNResult;
        }

        public async Task<SolicitorDossierShipment> SolicitorDossierShipmentRegisterAsync(SolicitorDossierShipmentResponse DTO, SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Obtenemos los datos del expedientewrapper
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SolicitorDossierShipmentResponse>(json)!;

            //creacion del Objeto
            SolicitorDossierShipmentContent content = new SolicitorDossierShipmentContent()
            {
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                SolicitorId = DTO.Content.SolicitorId.Id
            };

            SolicitorDossierShipment solicitorDossierShipment = new SolicitorDossierShipment()
            {
                Type = "EntregaExpedienteNotario",
                Content = content,
                ContentsHistory = new List<ContentVersion>(),
                ProcessesHistory = new List<Process>(),
                State = "creado",
                AttachedUrls = url2
            };

            await _documentsCollection.InsertOneAsync(solicitorDossierShipment);

            //actualizacion de expediente
            Dossier dossier = new Dossier();
            DossierDocument dossierDocument = new DossierDocument();
            dossierDocument.Index = 7;
            dossierDocument.DocumentId = solicitorDossierShipment.Id;
            dossierDocument.Type = "EntregaExpedienteNotario";
            dossierDocument.CreationDate = DateTime.UtcNow.AddHours(-5);
            dossierDocument.ExcessDate = DateTime.UtcNow.AddHours(-5).AddDays(5);
            dossierDocument.DelayDate = null;
            dossier = await UpdateDossierAsync(dossierDocument, dossierWrapper.Id);

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("estado", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            return solicitorDossierShipment;
        }

        public async Task<Document> ModifyStateAsync(Evaluation document, string docId)
        {
            var filter = Builders<Document>.Filter.Eq("id", docId);
            var update = Builders<Document>.Update
                .Set("evaluacion.resultado", document.Result)
                .Set("evaluacion.evaluaciones", document.Evaluations);
            return await _documentsCollection.FindOneAndUpdateAsync<Document>(filter, update);
            //BandejaDocumento bandejaDocumento = new BandejaDocumento();
            //bandejaDocumento.idexpediente = documento.idexpediente;
            //bandejaDocumento.iddocumento = documento.id;

            //UpdateDefinition<Bandeja> updateBandejaD = Builders<Bandeja>.Update.Pull("bandejaentrada", bandejaDocumento);
            //_bandejas.UpdateOne(band => band.usuario == documento.idusuario, updateBandejaD);

            //UpdateDefinition<Bandeja> updateBandejaI = Builders<Bandeja>.Update.Push("bandejasalida", bandejaDocumento);
            //_bandejas.UpdateOne(band => band.usuario == documento.idusuario, updateBandejaI);
        }

        public async Task<Document> GenerateDocumentAsync(GenerateDocumentRequest document)
        {
            Document doc = new Document();
            DocumentTray documentTray = new DocumentTray();
            documentTray.DossierId = document.DossierId;
            //bandejaDocumento.iddocumento = documento.iddocumento;
            documentTray.DocumentId = document.PreviousDocumentId;

            UpdateDefinition<Tray> updateTrayD = Builders<Tray>.Update.Pull("bandejaentrada", documentTray);
            await _trayCollection.UpdateOneAsync(t => t.User == document.UserId, updateTrayD);

            documentTray.DocumentId = document.DocumentId;

            UpdateDefinition<Tray> updateTrayI = Builders<Tray>.Update.Push("bandejasalida", documentTray);
            await _trayCollection.UpdateOneAsync(t => t.User == document.UserId, updateTrayI);

            ContentVersion contentVersion = new ContentVersion();

            contentVersion.Version = 1;
            contentVersion.Url = DateTime.UtcNow.AddHours(-5).ToString();

            var UpdateFilter = Builders<Document>.Update
                                                       .Set("contenido.codigo", document.Code)
                                                       .Set("contenido.firma", document.Sign)
                                                       .Set("contenido.urlGenerado", document.GeneratedURL)
                                                       .Push("historialcontenido", contentVersion);

            var UpdateQuery = Builders<Document>.Filter.Eq("id", document.DocumentId);

            await _documentsCollection.UpdateOneAsync(UpdateQuery, UpdateFilter);

            return doc;
        }

        public async Task<Document> ModifyStateDocumentAsync(DocumentRequest document)
        {
            var filter = Builders<Document>.Filter.Eq("id", document.Id);
            var update = Builders<Document>.Update
                .Set("estado", document.State);

            return await _documentsCollection.FindOneAndUpdateAsync<Document>(filter, update);
        }

        public async Task<SolicitorDesignationDocument> UpdateDocumentODNAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo OficioDesignacionNotarioDTO
            SolicitorDesignationDocumentResponse DTO = new SolicitorDesignationDocumentResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SolicitorDesignationDocumentResponse>(json)!;

            //Creacion de Obj OficioDesignacionNotario y registro en coleccion de documentos 
            SolicitorDesignationDocumentContent content = new SolicitorDesignationDocumentContent()
            {
                Title = DTO.content.Title,
                Description = DTO.content.Description,
                SolicitorAddress = DTO.content.SolicitorOfficeLocation,
                UserId = DTO.content.UserId,
                SolicitorId = DTO.content.SolicitorId.Id,
            };
            SolicitorDesignationDocument oficioDesignacionNotario = new SolicitorDesignationDocument();
            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.titulo", content.Title)
                .Set("contenido.descripcion", content.Description)
                .Set("contenido.lugaroficionotarial", content.SolicitorAddress)
                .Set("contenido.idusuario", content.UserId)
                .Set("contenido.idnotario", content.SolicitorId)
                .Set("urlanexo", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return oficioDesignacionNotario;
        }

        public async Task<Appeal> AppealDocumentUpdateAsync(SolicitorDesignationDocumentRegister dossierWrapper, string urlData, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            AppealResponse DTO = new AppealResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<AppealResponse>(json)!;

            //Creacion de Obj Apelacion y registro en coleccion de documentos 
            AppealContent content = new AppealContent()
            {
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                Url = urlData,
            };
            Appeal appeal = new Appeal();

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.titulo", content.Title)
                .Set("contenido.descripcion", content.Description)
                .Set("contenido.url", content.Url)
                .Set("urlanexo", url2);

            await _documentsCollection.UpdateOneAsync(filter, update);
            return appeal;
        }

        public async Task<DisciplinaryOpenness> DisciplinaryOpennessDocumentUpdateAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            DisciplinaryOpennessResponse DTO = new DisciplinaryOpennessResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<DisciplinaryOpennessResponse>(json)!;

            //Listas de participantes a string
            List<String> participantsList = new List<string>();
            foreach (Participant part in DTO.Content.Participants)
            {
                participantsList.Add(part.Name);
            }

            //Listas de hechos a string
            List<String> chargedDeedsList = new List<string>();
            foreach (Deed part in DTO.Content.ChargedDeeds)
            {
                chargedDeedsList.Add(part.Description);
            }

            //Creacion de Obj y registro en coleccion de documentos
            DisciplinaryOpennessContent content = new DisciplinaryOpennessContent()
            {
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                ComplainantName = DTO.Content.Complainant,
                AudiencePlace = DTO.Content.AudienceLocation,
                SolicitorId = DTO.Content.SolicitorId.Id,
                FiscalId = DTO.Content.ProsecutorId,
                Participants = participantsList,
                ImputedFacts = chargedDeedsList,
                AudienceStartDate = DTO.Content.AudienceStartDate,
                AudienceEndDate = DTO.Content.AudienceEndDate
            };
            DisciplinaryOpenness disciplinaryOpenness = new DisciplinaryOpenness();
            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.titulo", content.Title)
                .Set("contenido.descripcion", content.Description)
                .Set("contenido.nombredenunciante", content.ComplainantName)
                .Set("contenido.lugaraudiencia", content.AudiencePlace)
                .Set("contenido.idnotario", content.SolicitorId)
                .Set("contenido.idfiscal", content.FiscalId)
                .Set("contenido.participantes", content.Participants)
                .Set("contenido.hechosimputados", content.ImputedFacts)
                .Set("contenido.fechainicioaudiencia", content.AudienceStartDate)
                .Set("contenido.fechafinaudiencia", content.AudienceEndDate)
                .Set("urlanexo", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return disciplinaryOpenness;
        }

        public async Task<SignConclusion> UpdateSignConclusionDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            SignConclusionResponse DTO = new SignConclusionResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SignConclusionResponse>(json)!;
            SignConclusion documentCF = new SignConclusion();

            //Creacion de Obj y registro en coleccion de documentos 
            SignConclusionContent content = new SignConclusionContent()
            {
                PublicDeedId = DTO.Content.PublicDeedId.Id,
                SolicitorId = DTO.Content.SolicitorId.Id,
                ClientId = DTO.Content.ClientId.Id,
                TotalSheets = DTO.Content.PageQuantity,
                Price = DTO.Content.Price * 30
            };
            SignConclusion signConclusion = new SignConclusion();
            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.idescriturapublica", content.PublicDeedId)
                .Set("contenido.idnotario", content.PublicDeedId)
                .Set("contenido.idcliente", content.ClientId)
                .Set("contenido.cantidadfoja", content.TotalSheets)
                .Set("contenido.precio", content.Price)
                .Set("urlanexo", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return signConclusion;
        }

        public async Task<Dictum> UpdateDictumDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            DictumResponse DTO = new DictumResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<DictumResponse>(json)!;

            //Listas de participantes a string
            List<String> observationsList = new List<string>();
            foreach (Observations obs in DTO.Content.Observations)
            {
                observationsList.Add(obs.Description);
            }

            //Listas de hechos a string
            List<String> recomendationsList = new List<string>();
            foreach (Recomendations rec in DTO.Content.Recomendations)
            {
                recomendationsList.Add(rec.Description);
            }

            //Creacion de Obj y registro en coleccion de documentos 
            DictumContent contentD = new DictumContent()
            {
                ComplainantName = DTO.Content.ComplainantName,
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                Conclusion = DTO.Content.Conclusion,
                Observations = observationsList,
                Recommendations = recomendationsList
            };
            Dictum dictum = new Dictum();

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.titulo", contentD.Title)
                .Set("contenido.descripcion", contentD.Description)
                .Set("contenido.nombredenunciante", contentD.ComplainantName)
                .Set("contenido.conclusion", contentD.Conclusion)
                .Set("contenido.observaciones", contentD.Observations)
                .Set("contenido.recomendaciones", contentD.Recommendations)
                .Set("urlanexo", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return dictum;

        }

        public async Task<BPNDocument> UpdateBPNOfficeDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            BPNOfficeResponse DTO = new BPNOfficeResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<BPNOfficeResponse>(json)!;

            List<String> listaOtorgantes = new List<string>();
            foreach (BPNGranter obs in DTO.Content.Granters)
            {
                listaOtorgantes.Add(obs.Name);
            }

            //Creacion de Obj y registro en coleccion de documentos 
            BPNDocumentContent content = new BPNDocumentContent()
            {
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                ClientId = DTO.Content.ClientId.Id,
                DocumentAddress = DTO.Content.DocumentAddress,
                SolicitorId = DTO.Content.SolicitorId.Id,
                JuridicalAct = DTO.Content.LegalAct,
                ProtocolType = DTO.Content.ProtocolType,
                Grantors = listaOtorgantes
            };
            BPNDocument oficioBPN = new BPNDocument();
            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.titulo", content.Title)
                .Set("contenido.descripcion", content.Description)
                .Set("contenido.idcliente", content.ClientId)
                .Set("contenido.direccionoficio", content.DocumentAddress)
                .Set("contenido.idnotario", content.SolicitorId)
                .Set("contenido.actojuridico", content.JuridicalAct)
                .Set("contenido.tipoprotocolo", content.ProtocolType)
                .Set("contenido.otorgantes", content.Grantors)
                .Set("urlanexo", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return oficioBPN;
        }

        public async Task<Resolution> UpdateResolutionDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, string urlData, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            ResolutionResponse DTO = new ResolutionResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<ResolutionResponse>(json)!;

            //Listas de participantes a string
            List<String> listaParticipantes = new List<string>();
            foreach (Participant part in DTO.Content.Participants)
            {
                listaParticipantes.Add(part.Name);
            }

            //Creacion de Obj y registro en coleccion de documentos 
            ResolutionContent content = new ResolutionContent()
            {
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                Sanction = DTO.Content.Penalty,
                AudienceStartDate = DTO.Content.AudienceStartDate,
                AudienceEndDate = DTO.Content.AudienceEndDate,
                Url = urlData,
                Participants = listaParticipantes

            };
            Resolution resolution = new Resolution();
            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.titulo", content.Title)
                .Set("contenido.descripcion", content.Description)
                .Set("contenido.sancion", content.Sanction)
                .Set("contenido.fechainicioaudiencia", content.AudienceStartDate)
                .Set("contenido.fechafinaudiencia", content.AudienceEndDate)
                .Set("contenido.url", content.Url)
                .Set("contenido.participantes", content.Participants)
                .Set("urlanexo", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return resolution;
        }

        public async Task UpdateSENDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            //Deserealizacion de Obcject a tipo DTO
            SolicitorDossierRequestResponse DTO = new SolicitorDossierRequestResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SolicitorDossierRequestResponse>(json)!;

            //Creacion de Obj y registro en coleccion de documentos 
            SolicitorDossierRequestContent content = new SolicitorDossierRequestContent()
            {
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                SolicitorId = DTO.Content.SolicitorId.Id
            };

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.titulo", content.Title)
                .Set("contenido.descripcion", content.Description)
                .Set("contenido.idnotario", content.SolicitorId);
            await _documentsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<BPNResult> UpdateBPNResultDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            BPNResultResponse DTO = new BPNResultResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<BPNResultResponse>(json)!;

            //Creacion de Obj y registro en coleccion de documentos 
            BPNResultContent content = new BPNResultContent()
            {
                TotalSheets = DTO.Content.PageQuantity,
                Cost = DTO.Content.Cost,
                PublicDeedId = DTO.Content.PublicDeedId.Id
            };
            BPNResult result = new BPNResult();

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("contenido.cantidadfoja", content.TotalSheets)
                .Set("contenido.costo", content.Cost)
                .Set("contenido.idescriturapublica", content.PublicDeedId)
                .Set("urlanexo", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return result;
        }

        public async Task UpdateInitialRequestDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            //Deserealizacion de Obcject a tipo DTO
            InitialRequestResponse DTO = new InitialRequestResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<InitialRequestResponse>(json)!;
            //Creacion de Obj y registro en coleccion de documentos 
            InitialRequestContent content = new InitialRequestContent()
            {

                Title = DTO.Content.Title,
                Description = DTO.Content.Description
            };

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("estado", "modificado")
                .Set("contenido.titulo", DTO.Content.Title)
                .Set("contenido.descripcion", DTO.Content.Description);
            await _documentsCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateEENDocumentAsync(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            //Deserealizacion de Obcject a tipo DTO
            SolicitorDossierShipmentResponse DTO = new SolicitorDossierShipmentResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SolicitorDossierShipmentResponse>(json)!;

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("estado", "modificado")
                .Set("contenido.titulo", DTO.Content.Title)
                .Set("contenido.descripcion", DTO.Content.Description)
                .Set("contenido.idnotario", DTO.Content.SolicitorId.Id);

            await _documentsCollection.UpdateOneAsync(filter, update);
        }
        public async Task UpdateInitialRequestStateAsync(SolicitorDesignationDocumentRegister dossierWrapper)
        {
            //Deserealizacion de Obcject a tipo DTO
            InitialRequestResponse DTO = new InitialRequestResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<InitialRequestResponse>(json)!;

            InitialRequest solicitud = new InitialRequest()
            {
                State = DTO.State
            };

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("estado", DTO.State);
            await _documentsCollection.UpdateOneAsync(filter, update);
        }
        #region private methods
        private static BsonDocument[] GetDisciplinaryOpennessPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var lookupAggregation = GetSolicitorsLookUpPipeline();

            var unwinAggregation = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$tipo" },
                { "state", "$estado" },
                { "contentsHistory", "$historialcontenido" },
                { "processesHistory", "$historialproceso" },
                { "attachedUrls", "$urlanexo" },
                { "content", new BsonDocument()
                {
                    { "solicitor", "$solicitors" },
                    { "fiscalId", "$contenido.idfiscal" },
                    { "complainantName", "$contenido.nombredenunciante" },
                    { "title", "$contenido.titulo" },
                    { "description", "$contenido.descripcion" },
                    { "audienceStartDate", "$contenido.fechainicioaudiencia" },
                    { "audienceEndDate", "$contenido.fechafindaudiencia" },
                    { "participants", "$contenido.participantes" },
                    { "audiencePlace", "$contenido.lugaraudiencia" },
                    { "imputedFacts", "$contenido.hechosimputados" },
                    { "url", "$contenido.url" }
                } }
            });

            return new BsonDocument[] { matchAggregation, lookupAggregation, unwinAggregation, projectAggregation };
        }

        private static BsonDocument[] GetSignConclusionPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var publicDeedLookUpPipeline = GetPublicDeedLookUpPipeline();

            var publicDeedUnwindPipeline = MongoDBAggregationExtension.UnWind(new("$publicDeeds"));

            var solicitorLookUpPipeline = GetSolicitorsLookUpPipeline();

            var solicitorUnwindPipeline = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var clientLookUpPipeline = GetClientLookUpPipeline();

            var clientUnwindPipeline = MongoDBAggregationExtension.UnWind(new("$clients"));

            var projectPipeline = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$tipo" },
                { "state", "$estado" },
                { "contentsHistory", "$historialcontenido" },
                { "processesHistory", "$historialproceso" },
                { "attachedUrls", "$urlanexo" },
                { "content", new BsonDocument()
                {
                    { "client", "$clients" },
                    { "solicitor", "$solicitors" },
                    { "publicDeed", "$publicDeeds" },
                    { "price", "$contenido.precio" },
                    { "totalSheets", "$contenido.cantidadfoja" }
                } }
            });

            return new BsonDocument[] { matchAggregation, publicDeedLookUpPipeline, publicDeedUnwindPipeline,
                solicitorLookUpPipeline, solicitorUnwindPipeline, clientLookUpPipeline, clientUnwindPipeline, projectPipeline };
        }

        private static BsonDocument[] GetSolicitorDossierShipmentPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var lookUpPipeline = GetSolicitorsLookUpPipeline();

            var unwindPipeline = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var projectPipeline = MongoDBAggregationExtension.Project(new Dictionary<string, BsonValue>()
            {
                { "type", "$tipo" },
                { "state", "$estado" },
                { "contentsHistory", "$historialcontenido" },
                { "processesHistory", "$historialproceso" },
                { "attachedUrls", "$urlanexo" },
                { "content", new BsonDocument()
                {
                    { "title", "$contenido.titulo" },
                    { "description", "$contenido.descripcion" },
                    { "solicitor", "$solicitors" }
                } }
            });

            return new BsonDocument[] { matchAggregation, lookUpPipeline, unwindPipeline, projectPipeline };
        }

        private static BsonDocument[] GetSolicitorDossierRequestPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var lookUpPipeline = GetSolicitorsLookUpPipeline();

            var unwindPipeline = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var projectPipeline = MongoDBAggregationExtension.Project(new Dictionary<string, BsonValue>()
            {
                { "type", "$tipo" },
                { "state", "$estado" },
                { "contentsHistory", "$historialcontenido" },
                { "processesHistory", "$historialproceso" },
                { "attachedUrls", "$urlanexo" },
                { "content", new BsonDocument()
                {
                    { "title", "$contenido.titulo" },
                    { "description", "$contenido.descripcion" },
                    { "issueDate", "$contenido.fechaemision" },
                    { "solicitor", "$solicitors" }
                } }
            });

            return new BsonDocument[] { matchAggregation, lookUpPipeline, unwindPipeline, projectPipeline };
        }

        private static BsonDocument[] GetBPNResultPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var lookUpPipeline = GetPublicDeedLookUpPipeline();

            var unwindPipeline = MongoDBAggregationExtension.UnWind(new("$publicDeeds"));

            var projectPipeline = MongoDBAggregationExtension.Project(new Dictionary<string, BsonValue>()
            {
                { "type", "$tipo" },
                { "state", "$estado" },
                { "contentsHistory", "$historialcontenido" },
                { "processesHistory", "$historialproceso" },
                { "attachedUrls", "$urlanexo" },
                { "content", new BsonDocument()
                {
                    { "publicDeed", "$publicDeeds" },
                    { "totalSheets", "$contenido.cantidadfoja" },
                    { "cost", "$contenido.costo" },
                    { "status", "$contenido.estado" }
                } }
            });

            return new BsonDocument[] { matchAggregation, lookUpPipeline, unwindPipeline, projectPipeline };
        }

        private static BsonDocument GetPublicDeedLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "publicDeedId", MongoDBAggregationExtension.ObjectId("$contenido.idescriturapublica") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(
                    MongoDBAggregationExtension.Eq(new() { "$_id", "$$publicDeedId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("escrituraspublicas", letPipeline, lookUpPipeline, "publicDeeds"));
        }

        private static BsonDocument[] GetSolicitorDesignationPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var solicitorLookUpPipeline = GetSolicitorsLookUpPipeline();

            var solicitorUnWindPipeline = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var projectPipeline = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$tipo" },
                { "state", "$estado" },
                { "contentsHistory", "$historialcontenido" },
                { "processesHistory", "$historialproceso" },
                { "attachedUrls", "$urlanexo" },
                { "content", new BsonDocument()
                {
                    { "title", "$contenido.titulo" },
                    { "description", "$contenido.descripcion" },
                    { "solicitor", "$solicitors" },
                    { "userId", "$contenido.idusuario" },
                    { "solicitorAddress", "$contenido.lugaroficionotarial" },
                    { "realizationDate", "$contenido.fecharealizacion" }
                } }
            });

            return new BsonDocument[] { matchAggregation, solicitorLookUpPipeline, solicitorUnWindPipeline, projectPipeline };
        }

        private static BsonDocument[] GetBPNDocumentPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var solicitorLookUpPipeline = GetSolicitorsLookUpPipeline();

            var solicitorUnWindPipeline = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var clientLookUpPipeline = GetClientLookUpPipeline();

            var clientUnWindPipeline = MongoDBAggregationExtension.UnWind(new("$clients"));


            var projectPipeline = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$tipo" },
                { "state", "$estado" },
                { "contentsHistory", "$historialcontenido" },
                { "processesHistory", "$historialproceso" },
                { "attachedUrls", "$urlanexo" },
                { "content", new BsonDocument()
                {
                    { "title", "$contenido.titulo" },
                    { "description", "$contenido.descripcion" },
                    { "client", "$clients" },
                    { "solicitor", "$solicitors" },
                    { "juridicalAct", "$contentido.actojuridico" },
                    { "protocolType", "$contenido.tipoprotocolo" },
                    { "grantors", "$contenido.otorgantes" },
                    { "realizationDate", "$contenido.fecharealizacion" }
                } }

            });

            return new BsonDocument[] { matchAggregation, solicitorLookUpPipeline, solicitorUnWindPipeline, clientLookUpPipeline, clientUnWindPipeline, projectPipeline };
        }


        private static BsonDocument GetSolicitorsLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "solicitorId", MongoDBAggregationExtension.ObjectId("$contenido.idnotario") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$solicitorId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("notarios", letPipeline, lookUpPipeline, "solicitors"));
        }

        private static BsonDocument GetClientLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "clientId", MongoDBAggregationExtension.ObjectId("$contenido.idcliente") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$clientId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "clients"));
        }

        private static BsonDocument[] GetDocumentsPipeline(BsonDocument matchAggregation, BsonDocument[] monthPipelines)
        {

            var pipeline = new BsonDocument[] { matchAggregation };

            return pipeline.Union(monthPipelines).ToArray();
        }

        private static BsonDocument[] GetDocumentsByMonthAndAreaPipeline(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery)
        {
            return GetDocumentsPipeline(MongoDBAggregationExtension.Match(new BsonDocument("historialproceso.area", documentsByMonthAndAreaQuery.Area)), GetDocumentsByMonthPipeline(documentsByMonthAndAreaQuery));
        }

        private static BsonDocument[] GetExpiredDocumentsByMonthPipeline(DocumentsByMonthQuery documentsByMonthQuery)
        {
            return GetDocumentsPipeline(MongoDBAggregationExtension.Match(new BsonDocument("estado", "caducado")), GetDocumentsByMonthPipeline(documentsByMonthQuery));
        }

        private static BsonDocument[] GetDocumentsByStatePipeline(DocumentsByStateQuery documentsByStateQuery)
        {

            var matchAggregation = MongoDBAggregationExtension.Match(GetMatchAggregationPipeline(documentsByStateQuery));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "state", "$estado" },
                { "month", MongoDBAggregationExtension.Month("$fechacreacion") },
                { "type", "$tipo" }
            });

            var monthMatchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("month", documentsByStateQuery.Month));

            var groupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$type" },
                { "expired", MongoDBAggregationExtension.Sum(MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.Eq(new BsonArray() { "$state", "caducado" }), 1, 0)) },
                { "processed", MongoDBAggregationExtension.Sum(MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.In("$state", new BsonArray() { "procesado", "finalizado", "revisado" }), 1, 0)) },
                { "pending", MongoDBAggregationExtension.Sum(MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.In("$state", new BsonArray() { "pendiente", "creado", "modificado"  }), 1, 0)) }
            });

            var documentsByStateProject = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "documentType", "$_id" },
                { "expired", 1 },
                { "processed", 1 },
                { "pending", 1 }
            });

            return new BsonDocument[] { matchAggregation, projectAggregation, monthMatchAggregation, groupAggregation, documentsByStateProject };
        }

        private static Dictionary<string, BsonValue> GetMatchAggregationPipeline(DocumentsByStateQuery documentsByStateQuery)
        {
            var matchAggregationValues = new Dictionary<string, BsonValue>()
            {
                { "tipo",MongoDBAggregationExtension.NotIn(new List<BsonValue>() { "SolicitudInicial" }) }
            };

            if (!string.IsNullOrEmpty(documentsByStateQuery.Area)) matchAggregationValues.Add("historialproceso.area", documentsByStateQuery.Area);
            if (!string.IsNullOrEmpty(documentsByStateQuery.UserId)) matchAggregationValues.Add("historialproceso.idemisor", documentsByStateQuery.UserId);

            return matchAggregationValues;
        }

        private static BsonDocument[] GetDocumentsByMonthPipeline(DocumentsByMonthQuery documentsByMonthQuery)
        {

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 1 },
                { "tipo", 1 },
                { "month", MongoDBAggregationExtension.Month("$fechacreacion") }
            });

            var monthMatchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("month", documentsByMonthQuery.Month));

            var groupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$tipo" },
                { "quantity", MongoDBAggregationExtension.Sum(1) }
            });

            var documentsByMonthAndAreaProject = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "documentType", "$_id" },
                { "quantity", 1 }
            });

            return new[] { projectAggregation, monthMatchAggregation, groupAggregation, documentsByMonthAndAreaProject };
        }

        #endregion
    }
}
