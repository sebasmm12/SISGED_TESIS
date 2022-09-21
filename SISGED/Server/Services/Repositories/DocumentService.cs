using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Document.BPNDocument;
using SISGED.Shared.Models.Responses.Document.BPNResult;
using SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness;
using SISGED.Shared.Models.Responses.Document.SignConclusion;
using SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest;
using SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentService : IDocumentService
    {
        private readonly IMongoCollection<Document> _documentsCollection;
        public string CollectionName => "documentos";

        private readonly IMongoCollection<Dossier> _dossiersCollection;
        private string DossierCollectionName => "expedientes";

        public DocumentService(IMongoDatabase mongoDatabase)
        {
            _documentsCollection = mongoDatabase.GetCollection<Document>(CollectionName);
            _dossiersCollection = mongoDatabase.GetCollection<Dossier>(DossierCollectionName);

        }

        public async Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber)
        {
            var userRequestDocuments = await _dossiersCollection.Aggregate<UserRequestDocumentResponse>(GetUserRequestDocumentsPipeline(documentNumber)).ToListAsync();

            if (userRequestDocuments is null) throw new Exception($"No se pudo obtener las solicitudes iniciales del cliente con número de documento {documentNumber}");

            return userRequestDocuments;
        }

        public async Task<IEnumerable<UserRequestWithPublicDeedResponse>> GetUserRequestsWithPublicDeedAsync(string documentNumber)
        {
            var userRequestDocuments = await _dossiersCollection.Aggregate<UserRequestWithPublicDeedResponse>(GetUserRequestsWithPublicDeedPipeline(documentNumber)).ToListAsync();

            if (userRequestDocuments is null) throw new Exception($"No se pudo obtener las solicitudes iniciales del cliente con número de documento {documentNumber}");

            return userRequestDocuments;
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

            if (disciplinaryOpennessDocument is null) throw new Exception($"No se pudo obtener el aperturamiento disciplinario con identificador { documentId }");

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

        private static BsonDocument[] GetUserRequestsWithPublicDeedPipeline(string documentNumber)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("cliente.numerodocumento", documentNumber));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$tipo" },
                { "initialDocument", MongoDBAggregationExtension.ArrayElementAt(new List<BsonValue>() { "$documentos", 0 }) },
                { "lastDocument", MongoDBAggregationExtension.ArrayElementAt(new List<BsonValue>() { "$documentos", -1 })  }
            });

            var lookUpPipeline = GetUserRequestsWithPublicDeedLookUpPipeline();

            var documentsProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "type", 1  },
                { "initialDocument", MongoDBAggregationExtension.First(MongoDBAggregationExtension.Filter("$documents",
                    MongoDBAggregationExtension.Eq(new() { "$$documents._id", MongoDBAggregationExtension.ObjectId("$initialDocument.iddocumento") }), "documents"))},
                { "lastDocument", MongoDBAggregationExtension.First(MongoDBAggregationExtension.Filter("$documents",
                    MongoDBAggregationExtension.Eq(new() { "$$documents._id", MongoDBAggregationExtension.ObjectId("$lastDocument.iddocumento") }), "documents"))},
            });

            var publicDeedLookUpAggregation = GetDocumentLastPublicDeedLookUpPipeline();

            var userRequestsWithPublicDeedProject = GetUserRequestWithPublicDeedProjectPipeline();

            return new BsonDocument[] { matchAggregation, projectAggregation, lookUpPipeline,
                documentsProjectAggregation, publicDeedLookUpAggregation, userRequestsWithPublicDeedProject };

        }

        private static BsonDocument GetUserRequestWithPublicDeedProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", "$initialDocument._id" },
                { "type", 1 },
                { "state", "$initialDocument.estado" },
                { "content", "$initialDocument.contenido" },
                { "attachedUrls", "$initialDocument.urlanexo" },
                { "contentsHistory", "$initialDocument.historialcontenido" },
                { "processesHistory", "$initialDocument.historialproceso" },
                { "dossierUrl", MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.Eq(new() { MongoDBAggregationExtension.Size("$publicDeed"), 0 }),
                        "Ninguno", MongoDBAggregationExtension.First("$publicDeed.url")) },
            });

            return projectAggregation;
        }

        private static BsonDocument GetDocumentLastPublicDeedLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "publicDeedId", MongoDBAggregationExtension.ObjectId("$lastDocument.contenido.idescriturapublica") },
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(
                    MongoDBAggregationExtension.Eq(new() { "$_id", "$$publicDeedId" })))
            };


            return MongoDBAggregationExtension.Lookup(new("escrituraspublicas", letPipeline, lookUpPipeline, "publicDeed"));
        }

        private static BsonDocument GetUserRequestsWithPublicDeedLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "initialDocumentId", MongoDBAggregationExtension.ObjectId("$initialDocument.iddocumento") },
                { "lastDocumentId", MongoDBAggregationExtension.ObjectId("$lastDocument.iddocumento") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(
                    MongoDBAggregationExtension.In("$_id", new() { "$$initialDocumentId", "$$lastDocumentId" })))
            };


            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documents"));

        }

        private static BsonDocument[] GetUserRequestDocumentsPipeline(string documentNumber)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("cliente.numerodocumento", documentNumber));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "document", MongoDBAggregationExtension.ArrayElementAt(new List<BsonValue>() { "$documentos", 0 }) }
            });

            var lookUpAggregation = GetUserRequestsDocumentsLookUpPipeline();

            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var userRequestDocumentsProjection = MongoDBAggregationExtension.Project(new()
            {
                { "_id", "$documents._id" },
                { "type", "$documents.tipo"},
                { "state", "$documents.estado"},
                { "content", "$documents.contenido"},
                { "attachedUrls", "$documents.urlanexo"},
                { "contentsHistory", "$documents.historialcontenido"},
                { "processesHistory", "$documents.historialproceso"}
            });

            return new BsonDocument[] { matchAggregation, projectAggregation, lookUpAggregation, unWindAggregation, userRequestDocumentsProjection };
        }

        private static BsonDocument GetUserRequestsDocumentsLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "documentId", MongoDBAggregationExtension.ObjectId("$document.iddocumento") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                    .Eq(new () { "$_id", "$$documentId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documents"));
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
                { "expired", MongoDBAggregationExtension.Sum(MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.Eq(new BsonArray(){ "$state", "caducado" }), 1, 0)) },
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
                { "tipo", 1  },
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
                { "documentType", "$_id"  },
                { "quantity", 1 }
            });

            return new[] { projectAggregation, monthMatchAggregation, groupAggregation, documentsByMonthAndAreaProject };
        }

        #endregion
    }
}
