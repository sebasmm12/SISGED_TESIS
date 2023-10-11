using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Document;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Queries.UserDocument;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.BPNDocument;
using SISGED.Shared.Models.Responses.Document.BPNResult;
using SISGED.Shared.Models.Responses.Document.ComplaintRequest;
using SISGED.Shared.Models.Responses.Document.Dictum;
using SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness;
using SISGED.Shared.Models.Responses.Document.Resolution;
using SISGED.Shared.Models.Responses.Document.SignConclusion;
using SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Statistic;
using SISGED.Shared.Models.Responses.UserDocument;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentService : IDocumentService
    {
        private readonly IMongoCollection<Document> _documentsCollection;
        private readonly IMongoCollection<Tray> _trayCollection;
        private readonly IDossierService _dossierService;
        public string CollectionName => "documentos";
        public string TrayCollectionName => "bandejas";
        
        private readonly IEnumerable<string> annulmentInValidStates = new List<string>() { "evaluado", "anulado" };

        public DocumentService(IMongoDatabase mongoDatabase, IDossierService dossierService)
        {
            _documentsCollection = mongoDatabase.GetCollection<Document>(CollectionName);
            _trayCollection = mongoDatabase.GetCollection<Tray>(TrayCollectionName);

            _dossierService = dossierService;
        }

        public async Task<Document> AnnulDocumentAsync(string documentId, User user)
        {
            var annulmentProcess = new Process(user.Id, user.Id, "anulado", user.Rol);

            var updateDocumentState = Builders<Document>.Update.Set("state", "anulado")
                                                               .Push("processesHistory", annulmentProcess);

            var updatedDocument = await _documentsCollection.FindOneAndUpdateAsync(document => document.Id == documentId, updateDocumentState, new()
            {
                ReturnDocument = ReturnDocument.Before
            });

            if (updatedDocument is null) throw new Exception($"No se pudo anular el documento con identificador { documentId }");

            return updatedDocument;

        }

        public async Task<bool> VerifyDocumentAnnulmentAsync(string documentId)
        {
            var document = await GetDocumentAsync(documentId);

            return !annulmentInValidStates.Contains(document.State);
        }

        public async Task<IEnumerable<UserDocumentResponse>> GetDocumentsByUserAsync(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery)
        {
            var documents = await _documentsCollection.Aggregate<UserDocumentResponse>(GetPaginatedDocumentsByUserPipeline(userId, userDocumentPaginationQuery)).ToListAsync();

            if (documents is null) throw new Exception($"No se encontraron documentos del usuario con identificador {userId}");

            return documents;
        }

        public async Task<int> CountDocumentsByUserAsync(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery)
        {
            var totalDocumentsByUser = await _documentsCollection.Aggregate<UserDocumentCounterDTO>(GetTotalDocumentsByUserPipeline(userId, userDocumentPaginationQuery))
                .FirstAsync();

            return totalDocumentsByUser.Total;
        }

        public async Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber)
        {
            return await _dossierService.GetUserRequestDocumentsAsync(documentNumber);
        }

        public async Task<IEnumerable<UserRequestResponse>> GetUserRequestsWithPublicDeedAsync(UserRequestPaginationQuery userRequestPaginationQuery)
        {
            return await _dossierService.GetUserRequestsWithPublicDeedAsync(userRequestPaginationQuery);
        }

        public async Task<long> CountUserRequestAsync(string clientId)
        {
            return await _dossierService.CountUserRequestsAsync(clientId);
        }

        public async Task<BPNRequest> GetBPNRequestDocumentAsync(string documentId)
        {
            var bpnRequestDocument = await _documentsCollection.OfType<BPNRequest>().Find(document => document.Id == documentId).FirstAsync();

            if (bpnRequestDocument is null) throw new Exception($"No se pudo obtener la solicitud de búsqueda de protocolo notarial con el identificador {documentId}");

            return bpnRequestDocument;

        }

        public async Task<DictumInfoResponse> GetDictumDocumentAsync(string documentId)
        {
            var dictumDocument = await _documentsCollection.Aggregate<DictumInfoResponse>(GetDictumPipeline(documentId)).FirstAsync();

            if (dictumDocument is null) throw new Exception($"No se pudo obtener el dictamen con el identificador {documentId}");

            return dictumDocument;
        }

        public async Task<ResolutionInfoResponse> GetResolutionDocumentAsync(string documentId)
        {
            var resolutionDocument = await _documentsCollection.Aggregate<ResolutionInfoResponse>(GetResolutionPipeline(documentId)).FirstAsync();

            if (resolutionDocument is null) throw new Exception($"No se pudo obtener la resolución con el identificador {documentId}");

            return resolutionDocument;
        }

        public async Task<Appeal> GetAppealDocumentAsync(string documentId)
        {
            var appealDocument = await _documentsCollection.OfType<Appeal>().Find(document => document.Id == documentId).FirstAsync();

            if (appealDocument is null) throw new Exception($"No se pudo obtener el recurso de apelación con el identificador {documentId}");

            return appealDocument;
        }

        public async Task<ComplaintRequestInfoResponse> GetComplaintRequestDocumentAsync(string documentId)
        {
            var complaintRequest = await _documentsCollection.Aggregate<ComplaintRequestInfoResponse>(GetComplaintRequestPipeline(documentId)).FirstAsync();

            if (complaintRequest is null) throw new Exception($"No se pudo obtener la solicitud de denuncia con el identificador { documentId }");

            return complaintRequest;

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
            var updateDocumentProccess = Builders<Document>.Update.Push(document => document.ProcessesHistory, proccess)
                                                                  .Set("state", proccess.State);

            var updatedDocument = await _documentsCollection.UpdateOneAsync(document => document.Id == documentId, updateDocumentProccess);

            if (updatedDocument is null) throw new Exception($"No se pudo actualizar el historial del proceso del documento con identificador {documentId}");
        }

        public async Task<SolicitorDesignationDocument> SolicitorDesignationOfficeRegisterAsync(DossierWrapper dossierwrapper, List<string> url2)
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
            await _dossierService.FindOneAndUpdateAsync(dossierwrapper.Id, updateExpediente);
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

        public async Task<BPNDocument> RegisterBPNOfficeAsync(DossierWrapper dossierWrapper, List<string> url2, string url)
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
            Dossier expediente = await _dossierService.FindOneAndUpdateAsync(dossierWrapper.Id, updateExpediente);

            // TODO: IMPLEMENT A NEW SERVICE IN THE DOSSIER SERVICE

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("state", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);
            return documentoBPN;
        }

        public async Task<BPNRequest> RegisterBPNRquestAsync(DossierWrapper dossierWrapper, List<string> url2)
        {
            //Obtenemos los datos del expedientewrapper
            BPNRequestResponse document = new BPNRequestResponse();
            BPNRequestResponseContent listaotor = new BPNRequestResponseContent();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            document = JsonConvert.DeserializeObject<BPNRequestResponse>(json)!;

            //Solo para registrar nombre de grantors
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
                .Set("state", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            return solicitudBPN;
        }

        public async Task<ComplaintRequest> RegisterComplaintRequestAsync(ComplaintRequest complaintRequest)
        {
            await _documentsCollection.InsertOneAsync(complaintRequest);

            if (complaintRequest.Id is null) throw new Exception($"No se pudo registrar la solicitud de denuncia {complaintRequest.Content.Title}");

            return complaintRequest;
        }

        public async Task<SignExpeditionRequest> RegisterSignExpeditionRequestAsync(DossierWrapper dossierWrapepr, List<string> url2, string urlData)
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
                .Set("state", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            return documentSEF;
        }

        public async Task<InitialRequest> InitialRequestRegisterAsync(InitialRequest documentIR)
        {
            await _documentsCollection.InsertOneAsync(documentIR);
            return documentIR;
        }

        public async Task<SignConclusion> singConclusionERegisterAsync(DossierWrapper dossierWrapper, List<string> url2, string documentId)
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
            Dossier expediente = await _dossierService.FindOneAndUpdateAsync(dossierWrapper.Id, updateExpediente);
            // TODO: IMPLEMENT A NEW SERVICE IN THE DOSSIER SERVICE

            //Actulizar el documento anterior a revisado
            var filter = Builders<Document>.Filter.Eq("id", dossierWrapper.InputDocument);
            var update = Builders<Document>.Update
                .Set("state", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            //Actualizar el documento de solicitud a emitido, obtener el id de la solicitudBPN
            if (!string.IsNullOrEmpty(documentId))
            {
                var filterS = Builders<Document>.Filter.Eq("id", documentId);
                var updateS = Builders<Document>.Update
                    .Set("state", "emitido");
                await _documentsCollection.UpdateOneAsync(filterS, updateS);
            }
            return documentoDF;
        }

        public async Task<DisciplinaryOpenness> DisciplinaryOpennessRegisterAsync(DisciplinaryOpenness disciplinaryOpenness)
        {
            await _documentsCollection.InsertOneAsync(disciplinaryOpenness);

            if (disciplinaryOpenness.Id is null) throw new Exception($"No se pudo registrar el aperturamiento disciplinario {disciplinaryOpenness.Content.Title}");

            return disciplinaryOpenness;
        }

        public async Task<SolicitorDossierRequest> SolicitorDossierRequestRegisterAsync(SolicitorDossierRequest solicitorDossierRequest)
        {
            await _documentsCollection.InsertOneAsync(solicitorDossierRequest);

            if (solicitorDossierRequest.Id is null) throw new Exception($"No se pudo registrar la solicitud de expediente de notario {solicitorDossierRequest.Content.Title}");

            return solicitorDossierRequest;
        }

        public async Task<Dossier> UpdateDossierAsync(DossierDocument dossierDocument, string dossierId)
        {
            UpdateDefinition<Dossier> update = Builders<Dossier>.Update.Push("documentos", dossierDocument);

            Dossier dossier = await _dossierService.FindOneAndUpdateAsync(dossierId, update);

            return dossier;
        }

        public async Task<Dictum> RegisterDictumAsync(Dictum dictum)
        {
            await _documentsCollection.InsertOneAsync(dictum);

            if (dictum.Id is null) throw new Exception($"No se pudo registrar el dictamen {dictum.Content.Title}");

            return dictum;
        }

        public async Task<Resolution> ResolutionRegisterAsync(Resolution resolution)
        {
            await _documentsCollection.InsertOneAsync(resolution);

            if (resolution.Id is null) throw new Exception($"No se pudo registrar la resolución {resolution.Content.Title}");

            return resolution;
        }

        public async Task<SessionResolution> RegisterSessionResolutionAsync(SessionResolution sessionResolution)
        {
            await _documentsCollection.InsertOneAsync(sessionResolution);

            if (sessionResolution.Id is null) throw new Exception($"No se pudo registrar el dictamen {sessionResolution.Content.Title}");

            return sessionResolution;
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
                .Set("state", "revisado");
            await _documentsCollection.UpdateOneAsync(filter, update);

            //Actualizar el documento de solicitud a emitido, obtener el id de la solicitudBPN
            if (!string.IsNullOrEmpty(documentRequestId))
            {
                var filterS = Builders<Document>.Filter.Eq("id", documentRequestId);
                var updateS = Builders<Document>.Update
                    .Set("state", "emitido");
                await _documentsCollection.UpdateOneAsync(filterS, updateS);
            }

            return bPNResult;
        }

        public async Task<SolicitorDossierShipment> RegisterSolicitorDossierShipmentAsync(SolicitorDossierShipment solicitorDossierShipment)
        {
            await _documentsCollection.InsertOneAsync(solicitorDossierShipment);

            if (solicitorDossierShipment.Id is null) throw new Exception($"No se pudo registrar el envío de expediente de notario {solicitorDossierShipment.Content.Title}");

            return solicitorDossierShipment;
        }

        public async Task<Document> EvaluateDocumentAsync(DocumentEvaluationRequest documentEvaluationRequest, User user)
        {

            //var currentDocument = await GetDocumentAsync(documentEvaluationRequest.DocumentId);
            var eval = new DocumentEvaluation(user.Id, documentEvaluationRequest.IsApproved, documentEvaluationRequest.Comment, DateTime.UtcNow.AddHours(-5));
            var process = new Process(user.Id, user.Id, "evaluado", user.Rol, DateTime.UtcNow.AddHours(-5), DateTime.UtcNow.AddHours(-5));

            var updateFilter = Builders<Document>.Update
                                                       .Set("state", "evaluado")
                                                       .Push("processesHistory", process)
                                                       .Push("evaluations", eval);


            var updateQuery = Builders<Document>.Filter.Eq(document => document.Id, documentEvaluationRequest.DocumentId);

            var document = await _documentsCollection.FindOneAndUpdateAsync(updateQuery, updateFilter, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            return document;
        }

        public async Task<Document> GenerateDocumentAsync(DocumentGenerationDTO documentGenerationDTO)
        {
            var currentDocument = await GetDocumentAsync(documentGenerationDTO.DocumentId);

            var contentVersion = new ContentVersion(currentDocument.ContentsHistory.Count + 1, documentGenerationDTO.GeneratedURL);
            var process = new Process(documentGenerationDTO.UserId, documentGenerationDTO.UserId, "generado", documentGenerationDTO.RoleId);

            var updateFilter = Builders<Document>.Update
                                                       .Set("content.code", documentGenerationDTO.Code)
                                                       .Set("content.sign", documentGenerationDTO.Sign)
                                                       .Set("content.generatedUrl", documentGenerationDTO.GeneratedURL)
                                                       .Set("state", "generado")
                                                       .Push("contentsHistory", contentVersion)
                                                       .Push("processesHistory", process);


            var updateQuery = Builders<Document>.Filter.Eq(document => document.Id, documentGenerationDTO.DocumentId);

            var document = await _documentsCollection.FindOneAndUpdateAsync(updateQuery, updateFilter, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            return document;
        }

        public async Task<Document> ModifyStateDocumentAsync(DocumentRequest document)
        {
            var filter = Builders<Document>.Filter.Eq("id", document.Id);
            var update = Builders<Document>.Update
                .Set("estado", document.State);

            return await _documentsCollection.FindOneAndUpdateAsync<Document>(filter, update);
        }

        public async Task<SolicitorDesignationDocument> UpdateDocumentODNAsync(DossierWrapper dossierWrapper, List<string> url2)
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
                .Set("content.title", content.Title)
                .Set("content.description", content.Description)
                .Set("content.solicitorAddress", content.SolicitorAddress)
                .Set("content.userId", content.UserId)
                .Set("content.solicitorId", content.SolicitorId)
                .Set("attachedUrls", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return oficioDesignacionNotario;
        }

        public async Task<Appeal> AppealDocumentUpdateAsync(DossierWrapper dossierWrapper, string urlData, List<string> url2)
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
                .Set("content.title", content.Title)
                .Set("content.description", content.Description)
                .Set("content.url", content.Url)
                .Set("attachedUrls", url2);

            await _documentsCollection.UpdateOneAsync(filter, update);
            return appeal;
        }

        public async Task<DisciplinaryOpenness> DisciplinaryOpennessDocumentUpdateAsync(DossierWrapper dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            DisciplinaryOpennessResponse DTO = new DisciplinaryOpennessResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<DisciplinaryOpennessResponse>(json)!;

            //Listas de participants a string
            List<String> participantsList = DTO.Content.Participants;


            //Listas de hechos a string
            List<String> chargedDeedsList = DTO.Content.ChargedDeeds;

            //Creacion de Obj y registro en coleccion de documentos
            DisciplinaryOpennessContent content = new DisciplinaryOpennessContent()
            {
                Title = DTO.Content.Title,
                Description = DTO.Content.Description,
                AudiencePlace = DTO.Content.AudienceLocation,
                SolicitorId = DTO.Content.SolicitorId,
                Participants = participantsList,
                ImputedFacts = chargedDeedsList,
                AudienceStartDate = DTO.Content.AudienceStartDate,
                AudienceEndDate = DTO.Content.AudienceEndDate
            };
            DisciplinaryOpenness disciplinaryOpenness = new DisciplinaryOpenness();
            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("content.title", content.Title)
                .Set("content.description", content.Description)
                .Set("content.audiencePlace", content.AudiencePlace)
                .Set("content.solicitorId", content.SolicitorId)
                .Set("content.participants", content.Participants)
                .Set("content.imputedFacts", content.ImputedFacts)
                .Set("content.audienceStartDate", content.AudienceStartDate)
                .Set("content.audienceEndDate", content.AudienceEndDate)
                .Set("attachedUrls", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return disciplinaryOpenness;
        }

        public async Task<SignConclusion> UpdateSignConclusionDocumentAsync(DossierWrapper dossierWrapper, List<string> url2)
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
                .Set("content.publicDeedId", content.PublicDeedId)
                .Set("content.solicitorId", content.PublicDeedId)
                .Set("content.clientId", content.ClientId)
                .Set("content.totalSheets", content.TotalSheets)
                .Set("content.price", content.Price)
                .Set("attachedUrls", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return signConclusion;
        }

        public async Task<Dictum> UpdateDictumDocumentAsync(DossierWrapper dossierWrapper, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            DictumResponse DTO = new DictumResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<DictumResponse>(json)!;

            //Listas de participants a string
            List<String> observationsList = new List<string>();
            //foreach (Observations obs in DTO.Content.Observations)
            //{
            //    observationsList.Add(obs.Description);
            //}

            ////Listas de hechos a string
            //List<String> recomendationsList = new List<string>();
            //foreach (Recomendations rec in DTO.Content.Recomendations)
            //{
            //    recomendationsList.Add(rec.Description);
            //}

            //Creacion de Obj y registro en coleccion de documentos 
            DictumContent contentD = new DictumContent()
            {
                //ComplainantName = DTO.Content.ComplainantName,
                Title = DTO.Content.Title,
                //Description = DTO.Content.Description,
                Conclusion = DTO.Content.Conclusion,
                Observations = observationsList,
                //Recommendations = recomendationsList
            };
            Dictum dictum = new Dictum();

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("content.title", contentD.Title)
                //.Set("content.description", contentD.Description)
                //.Set("content.nombredenunciante", contentD.ComplainantName)
                .Set("content.conclusion", contentD.Conclusion)
                .Set("content.observations", contentD.Observations)
                .Set("content.recommendations", contentD.Recommendations)
                .Set("attachedUrls", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return dictum;

        }

        public async Task<BPNDocument> UpdateBPNOfficeDocumentAsync(DossierWrapper dossierWrapper, List<string> url2)
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
                .Set("content.title", content.Title)
                .Set("content.description", content.Description)
                .Set("content.clientId", content.ClientId)
                .Set("content.documentAddress", content.DocumentAddress)
                .Set("content.solicitorId", content.SolicitorId)
                .Set("content.juridicalAct", content.JuridicalAct)
                .Set("content.protocolType", content.ProtocolType)
                .Set("content.grantors", content.Grantors)
                .Set("attachedUrls", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return oficioBPN;
        }

        public async Task<Resolution> UpdateResolutionDocumentAsync(DossierWrapper dossierWrapper, string urlData, List<string> url2)
        {
            //Deserealizacion de Obcject a tipo DTO
            ResolutionResponse DTO = new ResolutionResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<ResolutionResponse>(json)!;

            //Listas de participants a string
            List<String> listaParticipantes = DTO.Content.Participants;

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
                .Set("content.title", content.Title)
                .Set("content.description", content.Description)
                .Set("content.sanction", content.Sanction)
                .Set("content.audienceStartDate", content.AudienceStartDate)
                .Set("content.audienceEndDate", content.AudienceEndDate)
                .Set("content.url", content.Url)
                .Set("content.participants", content.Participants)
                .Set("attachedUrls", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return resolution;
        }

        public async Task UpdateSENDocumentAsync(DossierWrapper dossierWrapper)
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
                SolicitorId = DTO.Content.SolicitorId
            };

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("content.title", content.Title)
                .Set("content.description", content.Description)
                .Set("content.solicitorId", content.SolicitorId);
            await _documentsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<BPNResult> UpdateBPNResultDocumentAsync(DossierWrapper dossierWrapper, List<string> url2)
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
                .Set("content.totalSheets", content.TotalSheets)
                .Set("content.cost", content.Cost)
                .Set("content.publicDeedId", content.PublicDeedId)
                .Set("attachedUrls", url2);
            await _documentsCollection.UpdateOneAsync(filter, update);
            return result;
        }

        public async Task UpdateInitialRequestDocumentAsync(DossierWrapper dossierWrapper)
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
                .Set("state", "modificado")
                .Set("content.title", DTO.Content.Title)
                .Set("content.description", DTO.Content.Description);
            await _documentsCollection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateEENDocumentAsync(DossierWrapper dossierWrapper)
        {
            //Deserealizacion de Obcject a tipo DTO
            SolicitorDossierShipmentResponse DTO = new SolicitorDossierShipmentResponse();
            var json = JsonConvert.SerializeObject(dossierWrapper.Document);
            DTO = JsonConvert.DeserializeObject<SolicitorDossierShipmentResponse>(json)!;

            var filter = Builders<Document>.Filter.Eq("id", DTO.Id);
            var update = Builders<Document>.Update
                .Set("state", "modificado")
                .Set("content.title", DTO.Content.Title)
                .Set("content.description", DTO.Content.Description)
                .Set("content.solicitorId", DTO.Content.SolicitorId);

            await _documentsCollection.UpdateOneAsync(filter, update);
        }
        public async Task UpdateInitialRequestStateAsync(DossierWrapper dossierWrapper)
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
                .Set("state", DTO.State);
            await _documentsCollection.UpdateOneAsync(filter, update);
        }
        #region private methods
        private static BsonDocument[] GetDictumPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var solicitorLookUpAggregation = GetSolicitorsLookUpPipeline();

            var solicitorUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var dossierLookUpPipelineAggregation = GetDossierLookUpPipeline();

            var dossierUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$dossiers"));
            
            var projectAggregation = GetDictumProjectPipeline();

            return new[] { matchAggregation, solicitorLookUpAggregation, solicitorUnwindAggregation, dossierLookUpPipelineAggregation,
                   dossierUnwindAggregation, projectAggregation };
        }

        private static BsonDocument[] GetComplaintRequestPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var documentTypeLookUpAggregation = GetDocumentTypeLookUpPipeline();

            var documentTypeUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$documentTypes"));

            var solicitorLookUpAggregation = GetSolicitorsLookUpPipeline();

            var solicitorUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var dossierLookUpPipelineAggregation = GetDossierLookUpPipeline();

            var dossierUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$dossiers"));

            var projectAggregation = GetComplaintRequestProjectPipeline();

            return new[] { matchAggregation, documentTypeLookUpAggregation, documentTypeUnwindAggregation, solicitorLookUpAggregation,
               solicitorUnwindAggregation, dossierLookUpPipelineAggregation, dossierUnwindAggregation, projectAggregation  };
        }

        private static BsonDocument[] GetSolicitorDossierShipmentPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var solicitorLookUpAggregation = GetSolicitorsLookUpPipeline();

            var solicitorUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var solicitorDossiersLookUpPipelineAggregation = GetSolicitorDossiersLookUpPipeline();

            var projectAggregation = GetSolicitorDossierShipmentProjectPipeline();

            return new[] { matchAggregation, solicitorLookUpAggregation, solicitorUnwindAggregation,
                solicitorDossiersLookUpPipelineAggregation, projectAggregation };
        }

        private static BsonDocument GetSolicitorDossierShipmentProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type"  },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory"  },
                { "attachedUrls", "$attachedUrls"  },
                { "state", "$state" },
                { "content", new BsonDocument()
                                .Add("code", "$content.code")
                                .Add("title", "$content.title")
                                .Add("description", "$content.description")
                                .Add("solicitor", new BsonDocument()
                                                    .Add("_id", "$solicitors._id")
                                                    .Add("name", "$solicitors.name")
                                                    .Add("lastName", "$solicitors.lastName")
                                                    .Add("solicitorOfficeName", "$solicitors.solicitorOffice.name")
                                                    .Add("email", "$solicitors.email")
                                                    .Add("address", "$solicitors.address"))
                                .Add("solicitorDossiers", "$solicitorDossiers")
                },
            });

            return projectAggregation;
        }

        private static BsonDocument GetDictumProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type"  },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory"  },
                { "attachedUrls", "$attachedUrls"  },
                { "state", "$state" },
                { "content", new BsonDocument()
                                .Add("code", "$content.code")
                                .Add("title", "$content.title")
                                .Add("conclusion", "$content.conclusion")
                                .Add("solicitor", new BsonDocument()
                                                    .Add("_id", "$solicitors._id")
                                                    .Add("name", "$solicitors.name")
                                                    .Add("lastName", "$solicitors.lastName")
                                                    .Add("solicitorOfficeName", "$solicitors.solicitorOffice.name")
                                                    .Add("email", "$solicitors.email")
                                                    .Add("address", "$solicitors.address"))
                                .Add("client", "$dossiers.client")
                                .Add("observations", "$content.observations")
                                .Add("recommendations", "$content.recommendations")
                },
            });

            return projectAggregation;
        }
        
        private static BsonDocument GetComplaintRequestProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type"  },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory"  },
                { "attachedUrls", "$attachedUrls"  },
                { "state", "$state" },
                { "content", new BsonDocument()
                                .Add("code", "$content.code")
                                .Add("title", "$content.title")
                                .Add("description", "$content.description")
                                .Add("solicitor", new BsonDocument()
                                                    .Add("_id", "$solicitors._id")
                                                    .Add("name", "$solicitors.name")
                                                    .Add("lastName", "$solicitors.lastName")
                                                    .Add("solicitorOfficeName", "$solicitors.solicitorOffice.name")
                                                    .Add("email", "$solicitors.email")
                                                    .Add("address", "$solicitors.address"))
                                .Add("client", "$dossiers.client")
                                .Add("complaintType", "$documentTypes.name")
                                .Add("deliveryDate", "$content.deliveryDate")
                },
            });

            return projectAggregation;
        }

        private static BsonDocument GetDisciplinaryOpennessProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type"  },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory"  },
                { "attachedUrls", "$attachedUrls"  },
                { "state", "$state" },
                { "content", new BsonDocument()
                                .Add("code", "$content.code")
                                .Add("title", "$content.title")
                                .Add("description", "$content.description")
                                .Add("solicitor", new BsonDocument()
                                                    .Add("_id", "$solicitors._id")
                                                    .Add("name", "$solicitors.name")
                                                    .Add("lastName", "$solicitors.lastName")
                                                    .Add("solicitorOfficeName", "$solicitors.solicitorOffice.name")
                                                    .Add("email", "$solicitors.email")
                                                    .Add("address", "$solicitors.address"))
                                .Add("client", "$dossiers.client")
                                .Add("audienceStartDate", "$content.audienceStartDate")
                                .Add("audienceEndDate", "$content.audienceEndDate")
                                .Add("audiencePlace", "$content.audiencePlace")
                                .Add("imputedFacts", "$content.imputedFacts")
                                .Add("participants", "$content.participants")
                },
            });

            return projectAggregation;
        }
        private static BsonDocument GetResolutionProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type"  },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory"  },
                { "attachedUrls", "$attachedUrls"  },
                { "state", "$state" },
                { "content", new BsonDocument()
                                .Add("code", "$content.code")
                                .Add("title", "$content.title")
                                .Add("description", "$content.description")
                                .Add("solicitor", new BsonDocument()
                                                    .Add("_id", "$solicitors._id")
                                                    .Add("name", "$solicitors.name")
                                                    .Add("lastName", "$solicitors.lastName")
                                                    .Add("solicitorOfficeName", "$solicitors.solicitorOffice.name")
                                                    .Add("email", "$solicitors.email")
                                                    .Add("address", "$solicitors.address"))
                                .Add("client", "$dossiers.client")
                                .Add("audienceStartDate", "$content.audienceStartDate")
                                .Add("audienceEndDate", "$content.audienceEndDate")
                                .Add("sanction", "$sanctionType.name")
                                .Add("participants", "$content.participants")
                },
            });

            return projectAggregation;
        }
        
        private static BsonDocument GetSolicitorDossierRequestProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type"  },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory"  },
                { "attachedUrls", "$attachedUrls"  },
                { "state", "$state" },
                { "content", new BsonDocument()
                                .Add("code", "$content.code")
                                .Add("title", "$content.title")
                                .Add("description", "$content.description")
                                .Add("solicitor", new BsonDocument()
                                                    .Add("_id", "$solicitors._id")
                                                    .Add("name", "$solicitors.name")
                                                    .Add("lastName", "$solicitors.lastName")
                                                    .Add("solicitorOfficeName", "$solicitors.solicitorOffice.name")
                                                    .Add("email", "$solicitors.email")
                                                    .Add("address", "$solicitors.address"))
                                .Add("client", "$dossiers.client")
                                .Add("issueDate", "$content.issueDate")
                },
            });

            return projectAggregation;
        }

        private static BsonDocument GetDocumentTypeLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "documentTypeId", MongoDBAggregationExtension.ObjectId("$content.complaintType") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$documentTypeId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("tipoDocumentos", letPipeline, lookUpPipeline, "documentTypes"));
        }


        private static BsonDocument[] GetPaginatedDocumentsByUserPipeline(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery)
        {
            var aggregations = GetDocumentsByUserPipeline(userId, userDocumentPaginationQuery).ToList();

            aggregations.Add(MongoDBAggregationExtension.Sort(new BsonDocument("creationDate", -1)));
            aggregations.Add(MongoDBAggregationExtension.Skip(userDocumentPaginationQuery.Page * userDocumentPaginationQuery.PageSize));
            aggregations.Add(MongoDBAggregationExtension.Limit(userDocumentPaginationQuery.PageSize));

            return aggregations.ToArray();
        }

        private static BsonDocument[] GetTotalDocumentsByUserPipeline(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery)
        {
            var aggregations = GetDocumentsByUserPipeline(userId, userDocumentPaginationQuery);

            var countAggregation = MongoDBAggregationExtension.Count("total");

            return aggregations.Concat(new[] { countAggregation }).ToArray();
        }

        private static BsonDocument[] GetDocumentsByUserPipeline(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery)
        {

            var aggregations = new List<BsonDocument>()
            {
                 GetDocumentsByUserMatchAggregation(userId, userDocumentPaginationQuery),
                 GetDossierLookUpPipeline(),
                 MongoDBAggregationExtension.UnWind(new("$dossiers"))
            };

            aggregations.AddRange(GetDocumentsByUserPipelineAggregation(userDocumentPaginationQuery).ToList());

            aggregations.Add(GetDocumentsByUserProjectPipeline());

            return aggregations.ToArray();
        }

        private static BsonDocument GetDocumentsByUserProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", 1 },
                { "contentsHistory", 1 },
                { "processesHistory", 1 },
                { "attachedUrls", 1 },
                { "state", 1 },
                { "creationDate", 1 },
                { "content", 1 },
                { "client", "$dossiers.client" },
                { "dossierType", "$dossiers.type" }
            });

            return projectAggregation;
        }

        private static BsonDocument GetDocumentsByUserMatchAggregation(string userId, UserDocumentPaginationQuery userDocumentPaginationQuery)
        {
            var matchedElements = new Dictionary<string, BsonValue>()
            {
                { "processesHistory", MongoDBAggregationExtension.ElementMatch(new()
                {
                    { "state", "registrado" },
                    { "senderId", userId }
                })}
            };

            var conditions = GetDocumentsByUserConditions();

            conditions.ForEach(condition =>
            {
                if (condition.Condition(userDocumentPaginationQuery)) matchedElements = condition.Result(matchedElements, userDocumentPaginationQuery);
            });

            var matchAggregation = MongoDBAggregationExtension.Match(matchedElements);

            return matchAggregation;
        }

        private static BsonDocument[] GetDocumentsByUserPipelineAggregation(UserDocumentPaginationQuery userDocumentPaginationQuery)
        {
            var pipelines = new List<BsonDocument>().ToArray();

            var conditions = GetDocumentsByUserPipelineConditions();

            conditions.ForEach(condition =>
            {
                if (condition.Condition(userDocumentPaginationQuery)) pipelines = condition.Result(pipelines, userDocumentPaginationQuery);
            });

            return pipelines;
        }
        
        private static List<FilterConditionDTO<UserDocumentPaginationQuery, BsonDocument[]>> GetDocumentsByUserPipelineConditions()
        {
            var documentsByUserConditions = new List<FilterConditionDTO<UserDocumentPaginationQuery, BsonDocument[]>>()
            {
                new()
                {
                    Condition = (userDocumentPaginationQuery) => !string.IsNullOrEmpty(userDocumentPaginationQuery.ClientName),
                    Result = (documentsByUserPipelines, userDocumentPaginationQuery) =>
                    {
                        var clientPipelines = GetClientSearcherPipeline(userDocumentPaginationQuery.ClientName!);

                        var result = documentsByUserPipelines.Concat(clientPipelines);

                        return result.ToArray();
                    }
                    
                },
                new()
                {
                    Condition = (userDocumentPaginationQuery) => ! string.IsNullOrEmpty(userDocumentPaginationQuery.DossierType),
                    Result = (documentsByUserPipelines, userDocumentPaginationQuery) =>
                    {
                        var dossierPipelines = GetDossierSearcherPipeline(userDocumentPaginationQuery.DossierType!);

                        var result = documentsByUserPipelines.Concat(dossierPipelines);

                        return result.ToArray();
                    }
                }
            };

            return documentsByUserConditions;
        }
        
        private static BsonDocument[] GetDossierSearcherPipeline(string dossierType)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("dossiers.tipo", dossierType));

            return new[] { matchAggregation };
        }

        private static BsonDocument GetSolicitorDossiersLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "solicitorDossierIds", "$content.solicitorDossiers" }
            };

            var lookUpPipeline = new BsonArray()
            {
               MongoDBAggregationExtension.Match(
                   MongoDBAggregationExtension.Expr(
                       MongoDBAggregationExtension.In(MongoDBAggregationExtension.ToString("$_id"), "$$solicitorDossierIds"))),
               
               MongoDBAggregationExtension.Project(new()
               {
                   { "issueDate", 0 },
                   { "solicitorId", 0 }
               })
            };

            return MongoDBAggregationExtension.Lookup(new("expedientesnotarios", letPipeline, lookUpPipeline, "solicitorDossiers"));
        }

        private static BsonDocument GetDossierLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "documentId", MongoDBAggregationExtension.ToString("$_id") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { MongoDBAggregationExtension.In("$$documentId", "$documentsHistory.documentId"), true })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "dossiers"));
        }

        private static BsonDocument GetSanctionLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "docTypeId", MongoDBAggregationExtension.ObjectId("$content.sanction") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$$docTypeId", "$_id" })))
            };

            return MongoDBAggregationExtension.Lookup(new("tipoDocumentos", letPipeline, lookUpPipeline, "sanctionType"));
        }

        private static BsonDocument[] GetClientSearcherPipeline(string clientName)
        {

            var addFieldsAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                { "fullName", MongoDBAggregationExtension.Concat(new List<BsonValue> { "$dossiers.client.name", " ", "$dossiers.client.lastName" }) }
            });

            var matchDictionary = new Dictionary<string, BsonValue>()
            {
                { "fullName", MongoDBAggregationExtension.Regex(clientName.Trim().ToLower() + ".*", "i") }
            };

            var matchAggregation = MongoDBAggregationExtension.Match(matchDictionary);

            var unsetAggregation = MongoDBAggregationExtension.UnSet(new List<BsonValue> { "fullName" });


            return new BsonDocument[] { addFieldsAggregation, matchAggregation, unsetAggregation };
        }

        private static List<FilterConditionDTO<UserDocumentPaginationQuery, Dictionary<string, BsonValue>>> GetDocumentsByUserConditions()
        {
            var documentByUserConditions = new List<FilterConditionDTO<UserDocumentPaginationQuery, Dictionary<string, BsonValue>>>()
            {
                new()
                {
                    Condition = (userDocumentPaginationQuery) => !string.IsNullOrEmpty(userDocumentPaginationQuery.Code),
                    Result = (matchedElements, userDocumentPaginationQuery) => {

                        string code = userDocumentPaginationQuery.Code!.Trim();
                        
                        matchedElements.Add("content.code", code);

                        return matchedElements;
                    } 
                },
                new()
                {
                    Condition = (userDocumentPaginationQuery) => !string.IsNullOrEmpty(userDocumentPaginationQuery.State),
                    Result = (matchedElements, userDocumentPaginationQuery) => {

                        matchedElements.Add("state", userDocumentPaginationQuery.State);

                        return matchedElements;
                    }                 
                },
                new()
                {
                    Condition = (userDocumentPaginationQuery) => userDocumentPaginationQuery.StartDate.HasValue,
                    Result = (matchedElements, userDocumentPaginationQuery) => {

                        matchedElements.Add("creationDate", MongoDBAggregationExtension.GreaterThanEquals(new BsonDateTime(userDocumentPaginationQuery.StartDate!.Value)));

                        return matchedElements;
                    }                               
                },
                new()
                {
                    Condition = (userDocumentPaginationQuery) => userDocumentPaginationQuery.EndDate.HasValue,
                    Result = (matchedElements, userDocumentPaginationQuery) => {

                        matchedElements.Add("creationDate", MongoDBAggregationExtension.LessThanEquals(new BsonDateTime(userDocumentPaginationQuery.EndDate!.Value)));

                        return matchedElements;
                    }                                             
                }
            };

            return documentByUserConditions;
        }
            
        private static BsonDocument[] GetDisciplinaryOpennessPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var solicitorLookUpAggregation = GetSolicitorsLookUpPipeline();

            var solicitorUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var dossierLookUpPipelineAggregation = GetDossierLookUpPipeline();

            var dossierUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$dossiers"));

            var projectAggregation = GetDisciplinaryOpennessProjectPipeline();

            return new[] { matchAggregation, solicitorLookUpAggregation,
               solicitorUnwindAggregation, dossierLookUpPipelineAggregation, dossierUnwindAggregation, projectAggregation  };
        }
        
        private static BsonDocument[] GetResolutionPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var solicitorLookUpAggregation = GetSolicitorsLookUpPipeline();

            var solicitorUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var dossierLookUpPipelineAggregation = GetDossierLookUpPipeline();

            var dossierUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$dossiers"));

            var sanctionLookUpPipelineAggregation = GetSanctionLookUpPipeline();

            var sanctionUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$sanctionType"));

            var projectAggregation = GetResolutionProjectPipeline();

            return new[] { matchAggregation, solicitorLookUpAggregation,
               solicitorUnwindAggregation,  dossierLookUpPipelineAggregation, dossierUnwindAggregation, sanctionLookUpPipelineAggregation, sanctionUnwindAggregation, projectAggregation  };
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
                { "type", "$type" },
                { "state", "$state" },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory" },
                { "attachedUrls", "$attachedUrls" },
                { "content", new BsonDocument()
                {
                    { "client", "$clients" },
                    { "solicitor", "$solicitors" },
                    { "publicDeed", "$publicDeeds" },
                    { "price", "$content.price" },
                    { "totalSheets", "$content.totalSheets" }
                } }
            });

            return new[] { matchAggregation, publicDeedLookUpPipeline, publicDeedUnwindPipeline,
                solicitorLookUpPipeline, solicitorUnwindPipeline, clientLookUpPipeline, clientUnwindPipeline, projectPipeline };
        }

        private static BsonDocument[] GetSolicitorDossierRequestPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var solicitorLookUpAggregation = GetSolicitorsLookUpPipeline();

            var solicitorUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$solicitors"));

            var dossierLookUpPipelineAggregation = GetDossierLookUpPipeline();

            var dossierUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$dossiers"));

            var projectAggregation = GetSolicitorDossierRequestProjectPipeline();

            return new[] { matchAggregation, solicitorLookUpAggregation,
               solicitorUnwindAggregation,  dossierLookUpPipelineAggregation, dossierUnwindAggregation, projectAggregation  };
        }

        private static BsonDocument[] GetBPNResultPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var lookUpPipeline = GetPublicDeedLookUpPipeline();

            var unwindPipeline = MongoDBAggregationExtension.UnWind(new("$publicDeeds"));

            var projectPipeline = MongoDBAggregationExtension.Project(new Dictionary<string, BsonValue>()
            {
                { "type", "$type" },
                { "state", "$state" },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory" },
                { "attachedUrls", "$attachedUrls" },
                { "content", new BsonDocument()
                {
                    { "publicDeed", "$publicDeeds" },
                    { "totalSheets", "$content.totalSheets" },
                    { "cost", "$content.cost" },
                    { "status", "$content.status" }
                } }
            });

            return new[] { matchAggregation, lookUpPipeline, unwindPipeline, projectPipeline };
        }

        private static BsonDocument GetPublicDeedLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "publicDeedId", MongoDBAggregationExtension.ObjectId("$content.publicDeedId") }
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
                { "type", "$type" },
                { "state", "$state" },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory" },
                { "attachedUrls", "$attachedUrls" },
                { "content", new BsonDocument()
                {
                    { "title", "$content.title" },
                    { "description", "$content.description" },
                    { "solicitor", "$solicitors" },
                    { "userId", "$content.userId" },
                    { "solicitorAddress", "$content.solicitorAddress" },
                    { "realizationDate", "$content.realizationDate" }
                } }
            });

            return new[] { matchAggregation, solicitorLookUpPipeline, solicitorUnWindPipeline, projectPipeline };
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
                { "type", "$type" },
                { "state", "$state" },
                { "contentsHistory", "$contentsHistory" },
                { "processesHistory", "$processesHistory" },
                { "attachedUrls", "$attachedUrls" },
                { "content", new BsonDocument()
                {
                    { "title", "$content.title" },
                    { "description", "$content.description" },
                    { "client", "$clients" },
                    { "solicitor", "$solicitors" },
                    { "juridicalAct", "$content.juridicalAct" },
                    { "protocolType", "$content.protocolType" },
                    { "grantors", "$content.grantors" },
                    { "realizationDate", "$content.realizationDate" }
                } }

            });

            return new[] { matchAggregation, solicitorLookUpPipeline, solicitorUnWindPipeline, clientLookUpPipeline, clientUnWindPipeline, projectPipeline };
        }


        private static BsonDocument GetSolicitorsLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "solicitorId", MongoDBAggregationExtension.ObjectId("$content.solicitorId") }
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
                { "clientId", MongoDBAggregationExtension.ObjectId("$content.clientId") }
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
            return GetDocumentsPipeline(MongoDBAggregationExtension.Match(new BsonDocument("processesHistory.area", documentsByMonthAndAreaQuery.Area)), GetDocumentsByMonthPipeline(documentsByMonthAndAreaQuery));
        }

        private static BsonDocument[] GetExpiredDocumentsByMonthPipeline(DocumentsByMonthQuery documentsByMonthQuery)
        {
            return GetDocumentsPipeline(MongoDBAggregationExtension.Match(new BsonDocument("state", "caducado")), GetDocumentsByMonthPipeline(documentsByMonthQuery));
        }

        private static BsonDocument[] GetDocumentsByStatePipeline(DocumentsByStateQuery documentsByStateQuery)
        {

            var matchAggregation = MongoDBAggregationExtension.Match(GetMatchAggregationPipeline(documentsByStateQuery));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "state", "$state" },
                { "month", MongoDBAggregationExtension.Month("$creationDate") },
                { "type", "$type" }
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

            return new[] { matchAggregation, projectAggregation, monthMatchAggregation, groupAggregation, documentsByStateProject };
        }

        private static Dictionary<string, BsonValue> GetMatchAggregationPipeline(DocumentsByStateQuery documentsByStateQuery)
        {
            var matchAggregationValues = new Dictionary<string, BsonValue>()
            {
                { "type",MongoDBAggregationExtension.NotIn(new List<BsonValue>() { "SolicitudInicial" }) }
            };

            if (!string.IsNullOrEmpty(documentsByStateQuery.Area)) matchAggregationValues.Add("processesHistory.area", documentsByStateQuery.Area);
            if (!string.IsNullOrEmpty(documentsByStateQuery.UserId)) matchAggregationValues.Add("processesHistory.senderId", documentsByStateQuery.UserId);

            return matchAggregationValues;
        }

        private static BsonDocument[] GetDocumentsByMonthPipeline(DocumentsByMonthQuery documentsByMonthQuery)
        {

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 1 },
                { "type", 1 },
                { "month", MongoDBAggregationExtension.Month("$creationDate") }
            });

            var monthMatchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("month", documentsByMonthQuery.Month));

            var groupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$type" },
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
