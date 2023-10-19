using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Document;
using SISGED.Shared.Models.Queries.Dossier;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Repositories
{
    public class DossierService : IDossierService
    {
        private readonly IMongoCollection<Dossier> _dossiersCollection;
        private readonly IMongoCollection<Document> _documentsCollection;
        private readonly ITrayService _trayService;
        private readonly IAssistantService _assistantService;

        public string DocumentsCollectionName => "documentos";

        public DossierService(
            IMongoDatabase mongoDatabase, 
            ITrayService trayService, 
            IAssistantService assistantService)
        {

            _dossiersCollection = mongoDatabase.GetCollection<Dossier>(CollectionName);
            _documentsCollection = mongoDatabase.GetCollection<Document>(DocumentsCollectionName);
            _trayService = trayService;
            _assistantService = assistantService;
        }

        public string CollectionName => "expedientes";

        public async Task<Dossier> DeleteDossierDocumentAsync(string documentId)
        {
            var currentDossier = await GetDossierByDocumentAsync(documentId);

            var dossierDocument = currentDossier.Documents.First(dossierDocument => dossierDocument.DocumentId == documentId);

            var dossierUpdate = Builders<Dossier>.Update.Pull("documents", dossierDocument);

            var updatedDossier = await _dossiersCollection.FindOneAndUpdateAsync(dossier => dossier.Id == currentDossier.Id, dossierUpdate, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            if (updatedDossier is null) throw new Exception($"No se pudo eliminar el documento con identificador {documentId} del expediente con identificador {currentDossier.Id}");

            return updatedDossier;
        }

        public async Task CreateDossierAsync(Dossier dossier)
        {
            await _dossiersCollection.InsertOneAsync(dossier);

            if (dossier.Id is null) throw new Exception("No se pudo registrar el expediente al sistema");
        }

        public async Task<IEnumerable<Dossier>> GetDossiersAsync()
        {
            var dossiers = await _dossiersCollection.Find(_ => true).ToListAsync();

            if (dossiers is null) throw new Exception("No se pudo obtener los expedientes registrados en el sistema");

            return dossiers;
        }

        public async Task<IEnumerable<DossierGanttDiagramResponse>> GetDossierGanttDiagramAsync(DossierGanttDiagramQuery dossierGanttDiagramQuery)
        {
            var dossiers = await _dossiersCollection.Aggregate<DossierGanttDiagramResponse>(GetDossierGanttDiagramPipeline(dossierGanttDiagramQuery.DocumentNumber)).ToListAsync();

            if (dossiers is null) throw new Exception($"No se pudo obtener el diagrama gantt de los expedientes del cliente con número de documento {dossierGanttDiagramQuery.DocumentNumber}");

            return dossiers;
        }

        public async Task<DossierLastDocumentResponse> RegisterDerivationAsync(DossierLastDocumentRequest dossierLastDocumentRequest, string userId)
        {
            Derivation dossierDerivation = dossierLastDocumentRequest.Derivation;

            dossierDerivation.DerivationDate = DateTime.UtcNow.AddHours(-5);

            dossierDerivation.ReceiverUser = userId;

            await UpdateDossierDerivationsAsync(dossierDerivation, dossierLastDocumentRequest.Id);

            var assistant = await _assistantService.GetAssistantByDossierAsync(dossierLastDocumentRequest.Id);

            await UpdateDossierGeneratedDocumentAsync(dossierLastDocumentRequest, assistant);

            var process = new Process(dossierDerivation.SenderUser, dossierDerivation.ReceiverUser, "derivado", dossierLastDocumentRequest.Derivation.OriginArea);

            var documentProcessUpdate = UpdateDocumentProcessAsync(process, dossierLastDocumentRequest.DocumentId);

            var usersTraysUpdate = UpdateUsersTraysAsync(assistant, dossierLastDocumentRequest, userId);

            await Task.WhenAll(usersTraysUpdate, documentProcessUpdate);

            return await GetDossiertLastDocument(dossierLastDocumentRequest.Id);
        }

        public async Task<Dossier> GetDossierAsync(string dossierId)
        {
            var dossier = await _dossiersCollection.Find(dossier => dossier.Id == dossierId).FirstOrDefaultAsync();

            if (dossier is null) throw new Exception($"No se pudo obtener el expediente con id {dossierId}");

            return dossier;
        }

        public async Task<Dossier> UpdateDossierForInitialRequestAsync(Dossier dossier)
        {
            var filter = Builders<Dossier>.Filter.Eq(dossierToUpdate => dossierToUpdate.Id, dossier.Id);
            var update = Builders<Dossier>.Update.Set(dossierToUpdate => dossierToUpdate.Type, dossier.Type)
                                                 .Set(dossierToUpdate => dossierToUpdate.State, dossier.State)
                                                 .Push(dossierToUpdate => dossierToUpdate.Documents, dossier.Documents.First())
                                                 .Push(dossierToUpdate => dossierToUpdate.DocumentsHistory, dossier.DocumentsHistory.First());

            var updatedDossier = await _dossiersCollection.FindOneAndUpdateAsync(filter, update, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            if (updatedDossier is null) throw new Exception($"No se pudo actualizar el expediente con identificador {dossier.Id}");

            return updatedDossier;
        }

        public async Task<IEnumerable<Dossier>> GetDossierByFiltersAsync(DossierHistoryQuery dossierHistoryQuery)
        {
            var filteredDossiers = await _dossiersCollection.Aggregate<Dossier>(GetFilteredDossiersPipeline(dossierHistoryQuery)).ToListAsync();

            if (filteredDossiers is null) throw new Exception("No se pudo obtener los expedientes mediante los filtros enviados");

            return filteredDossiers;
        }

        public async Task<IEnumerable<T>> ExecuteDossierAggregateAsync<T>(BsonDocument[] pipelines)
        {
            return await _dossiersCollection.Aggregate<T>(pipelines).ToListAsync();
        }

        public async Task<DossierResponse> GetDossierByIdAsync(string documentId)
        {
            Dossier dossier = await _dossiersCollection.Find(exp => exp.Id == documentId).FirstOrDefaultAsync();
            DossierResponse dTO = new DossierResponse
            {
                Client = dossier.Client,
                Derivations = dossier.Derivations,
                Documents = dossier.Documents,
                State = dossier.State,
                EndDate = dossier.EndDate,
                StartDate = dossier.StartDate,
                Id = dossier.Id,
                Type = dossier.Type
            };
            return dTO;
        }
        public async Task<IEnumerable<UserRequestDocumentResponse>> GetUserRequestDocumentsAsync(string documentNumber)
        {
            var userRequestDocuments = await _dossiersCollection.Aggregate<UserRequestDocumentResponse>(GetUserRequestDocumentsPipeline(documentNumber)).ToListAsync();

            if (userRequestDocuments is null) throw new Exception($"No se pudo obtener las solicitudes iniciales del cliente con número de documento {documentNumber}");

            return userRequestDocuments;
        }

        public async Task<IEnumerable<UserRequestResponse>> GetUserRequestsWithPublicDeedAsync(UserRequestPaginationQuery userRequestPaginationQuery)
        {
            var userRequestDocuments = await _dossiersCollection.Aggregate<UserRequestResponse>(GetUserRequestsWithPublicDeedPipeline(userRequestPaginationQuery)).ToListAsync();

            if (userRequestDocuments is null) throw new Exception($"No se pudo obtener las solicitudes iniciales del cliente con identificador {userRequestPaginationQuery.ClientId}");

            return userRequestDocuments;
        }

        public async Task<Dossier> FindOneAndUpdateAsync(string Id, UpdateDefinition<Dossier> update)
        {
            return await _dossiersCollection.FindOneAndUpdateAsync(x => x.Id == Id, update);
        }

        public async Task<long> CountUserRequestsAsync(string clientId)
        {
            long totalRequests = await _dossiersCollection.CountDocumentsAsync(dossier => dossier.Client.ClientId == clientId);

            return totalRequests;
        }
        public async Task<IEnumerable<DossierListResponse>> GetDossiersListAsync(UserDossierPaginationQuery paginationQuery)
        {
            var dossiers = await _dossiersCollection.Aggregate<DossierListResponse>(GetPaginatedDossiersPipeline(paginationQuery))
                .ToListAsync();

            if (dossiers is null) throw new Exception($"No se encontraron expedientes.");

            return dossiers;
        }

        public async Task<int> CountDossiersListAsync(UserDossierPaginationQuery paginationQuery)
        {
            var totalDossiers = await _dossiersCollection.Aggregate<UserDocumentCounterDTO>(GetTotalDossiersPipeline(paginationQuery))
                .FirstAsync();

            return totalDossiers.Total;
        }

        public async Task<Dossier> DenyDossierByDocumentAsync(string documentId)
        {
            var currentDossier = await GetDossierByDocumentAsync(documentId);

            var dossierUpdate = Builders<Dossier>.Update.Set("state", "rechazado");

            var updatedDossier = await _dossiersCollection.FindOneAndUpdateAsync(dossier => dossier.Id == currentDossier.Id, dossierUpdate, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            if (updatedDossier is null) throw new Exception($"No se pudo actualizar el estado del expediente con identificador {currentDossier.Id} en base al documento con identificador {documentId}");

            return updatedDossier;
        }

        #region private methods

        private async Task UpdateDossierGeneratedDocumentAsync(DossierLastDocumentRequest dossierLastDocumentRequest, Assistant assistant)
        {
            if (!assistant.IsLastStep())
                return;

            var document = await GetDocumentAsync(dossierLastDocumentRequest.DocumentId);

            var documentUrl = document
                                .ContentsHistory
                                .LastOrDefault()!
                                .Url;

            var dossierUpdate = Builders<Dossier>
                                    .Update
                                    .Set(dossier => dossier.DocumentUrl, documentUrl)
                                    .Set(dossier => dossier.EndDate, DateTime.UtcNow.AddHours(-5))
                                    .Set(dossier => dossier.State, "Finalizado");

            var updatedDossier = await _dossiersCollection.FindOneAndUpdateAsync(dossier => dossier.Id == dossierLastDocumentRequest.Id, dossierUpdate, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            if (updatedDossier is null) 
                throw new Exception($"No se pudo registrar la url del documento con expediente {dossierLastDocumentRequest.Id}");
        }

        private async Task<Document> GetDocumentAsync(string documentId)
        {
            var document = await _documentsCollection
                                    .Find(document => document.Id == documentId)
                                    .FirstAsync();

            return document;
        }

        private Task UpdateUsersTraysAsync(Assistant assistant, DossierLastDocumentRequest dossierLastDocumentRequest, string userId)
        {
            if (assistant.IsLastStep())
            {
                var updateDocumentTrayDTO = new UpdateDocumentTrayDTO(new(dossierLastDocumentRequest.Id, 
                                                                          dossierLastDocumentRequest.DocumentId), 
                                                                          dossierLastDocumentRequest.Derivation.SenderUser, "outputTray");

                return _trayService.PullDocumentTrayAsync(updateDocumentTrayDTO);
            }

            var updateTrayDTO = new UpdateTrayDTO(dossierLastDocumentRequest.Id, userId,
                dossierLastDocumentRequest.Derivation.SenderUser,
                dossierLastDocumentRequest.DocumentId);

            return _trayService.UpdateTrayForDerivationAsync(updateTrayDTO);
        }

        private async Task<Dossier> GetDossierByDocumentAsync(string documentId)
        {
            var dossierDocumentBuilder = Builders<DossierDocument>.Filter.Eq(dosssierDocument => dosssierDocument.DocumentId, documentId);

            var dossierBuilder = Builders<Dossier>.Filter.ElemMatch(dossier => dossier.Documents, dossierDocumentBuilder);

            var dossier = await _dossiersCollection.Find(dossierBuilder).FirstOrDefaultAsync();

            if (dossier is null) throw new Exception($"No se pudo encontrar el expediente que tiene el document con identificador {documentId} relacionado");

            return dossier;
        }

        private static BsonDocument[] GetUserRequestsWithPublicDeedPipeline(UserRequestPaginationQuery userRequestPaginationQuery)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("client.clientId", userRequestPaginationQuery.ClientId));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type" },
                { "initDate", "$startDate" },
                { "initialDocument", MongoDBAggregationExtension.ArrayElementAt(new List<BsonValue>() { "$documents", 0 }) },
                { "state", "$state" },
                { "endDate", "$endDate" },
                { "documentUrl", "$documentUrl" }
            });

            var lookUpPipeline = GetUserRequestsWithPublicDeedLookUpPipeline();

            var documentsUnWindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));
            
            var documentsProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "type", 1  },
                { "initDate", 1 },
                { "initialDocument", "$documents" },
                { "state", 1},
                { "endDate", 1 },
                { "documentUrl", 1 }
            });

            var sortAggregation = MongoDBAggregationExtension.Sort(new BsonDocument("initDate", -1));

            var skipAggregation = MongoDBAggregationExtension.Skip(userRequestPaginationQuery.Page * userRequestPaginationQuery.PageSize);

            var limitAggregation = MongoDBAggregationExtension.Limit(userRequestPaginationQuery.PageSize);

            return new BsonDocument[] { matchAggregation, projectAggregation, lookUpPipeline, documentsUnWindAggregation,
                documentsProjectAggregation,
                sortAggregation, skipAggregation, limitAggregation };
        }

        private static BsonDocument GetUserRequestWithPublicDeedProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", "$initialDocument._id" },
                { "initDate", 1 },
                { "type", 1 },
                { "state", "$initialDocument.state" },
                { "content", "$initialDocument.content" },
                { "attachedUrls", "$initialDocument.attachedUrls" },
                { "contentsHistory", "$initialDocument.contentsHistory" },
                { "processesHistory", "$initialDocument.processesHistory" },
                { "dossierUrl", MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.Eq(new() { MongoDBAggregationExtension.Size("$publicDeed"), 0 }),
                        "Ninguno", MongoDBAggregationExtension.First("$publicDeed.url")) },
            });

            return projectAggregation;
        }

        private static BsonDocument GetDocumentLastPublicDeedLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "publicDeedId", MongoDBAggregationExtension.ObjectId("$lastDocument.content.publicDeedId") },
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
                { "initialDocumentId", MongoDBAggregationExtension.ObjectId("$initialDocument.documentId") },
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(
                    MongoDBAggregationExtension.In("$_id", new() { "$$initialDocumentId" })))
            };


            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documents"));
        }

        private static BsonDocument[] GetUserRequestDocumentsPipeline(string documentNumber)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("client.documentNumber", documentNumber));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "document", MongoDBAggregationExtension.ArrayElementAt(new List<BsonValue>() { "$documents", 0 }) }
            });

            var lookUpAggregation = GetUserRequestsDocumentsLookUpPipeline();

            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var userRequestDocumentsProjection = MongoDBAggregationExtension.Project(new()
            {
                { "_id", "$documents._id" },
                { "type", "$documents.type"},
                { "state", "$documents.state"},
                { "content", "$documents.content"},
                { "attachedUrls", "$documents.attachedUrls"},
                { "contentsHistory", "$documents.contentsHistory"},
                { "processesHistory", "$documents.processesHistory"}
            });

            return new BsonDocument[] { matchAggregation, projectAggregation, lookUpAggregation, unWindAggregation, userRequestDocumentsProjection };
        }

        private static BsonDocument GetUserRequestsDocumentsLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "documentId", MongoDBAggregationExtension.ObjectId("$document.documentId") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                    .Eq(new () { "$_id", "$$documentId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documents"));
        }

        private async Task UpdateDocumentProcessAsync(Process proccess, string documentId)
        {
            var updateDocumentQuery = Builders<Document>.Filter.Eq(document => document.Id, documentId);

            var updateDocumentProccess = Builders<Document>.Update.Push("processesHistory", proccess)
                                                                  .Set("state", "derivado");

            var updatedDocument = await _documentsCollection.FindOneAndUpdateAsync(updateDocumentQuery, updateDocumentProccess);

            if (updatedDocument is null) throw new Exception($"No se pudo actualizar el historial del proceso del documento con identificador {documentId}");
        }

        private BsonDocument[] GetFilteredDossiersPipeline(DossierHistoryQuery dossierHistoryQuery)
        {
            var dossierConditions = GetDossierConditions();

            var dossierFilters = new BsonDocument();

            dossierConditions.ForEach(dossierCondition =>
            {
                if (dossierCondition.Condition(dossierHistoryQuery)) dossierFilters = dossierCondition.Result(dossierFilters, dossierHistoryQuery);
            });

            var matchAggregation = MongoDBAggregationExtension.Match(dossierFilters);

            var skipAggregation = MongoDBAggregationExtension.Skip(dossierHistoryQuery.Page * dossierHistoryQuery.QuantityPerPage);

            var limitAggregation = MongoDBAggregationExtension.Limit(dossierHistoryQuery.QuantityPerPage);

            return new BsonDocument[] { matchAggregation, skipAggregation, limitAggregation };

        }

        private List<DossierFiltersConditionDTO<BsonDocument>> GetDossierConditions()
        {
            var dossierConditions = new List<DossierFiltersConditionDTO<BsonDocument>>()
            {
                new()
                {
                    Condition = (DossierHistoryQuery dossierHistoryQuery) => !string.IsNullOrEmpty(dossierHistoryQuery.State),
                    Result = (BsonDocument dossiersFilter, DossierHistoryQuery dossierHistoryQuery) =>
                                           dossiersFilter.Add("state", MongoDBAggregationExtension.Regex(dossierHistoryQuery.State! + ".*", "i"))
                },
                new()
                {
                    Condition = (DossierHistoryQuery dossierHistoryQuery) => !string.IsNullOrEmpty(dossierHistoryQuery.Type),
                    Result = (BsonDocument dossiersFilter, DossierHistoryQuery dossierHistoryQuery) =>
                                            dossiersFilter.Add("type", MongoDBAggregationExtension.Regex(dossierHistoryQuery.Type! + ".*", "i"))
                },
                new()
                {
                    Condition = (DossierHistoryQuery dossierHistoryQuery) => !string.IsNullOrEmpty(dossierHistoryQuery.ClientName),
                    Result = (BsonDocument dossiersFilter, DossierHistoryQuery dossierHistoryQuery) =>
                                           dossiersFilter.Add("client.name", MongoDBAggregationExtension.Regex(dossierHistoryQuery.ClientName! + ".*", "i"))
                }
            };

            return dossierConditions;
        }

        private async Task UpdateDossierDerivationsAsync(Derivation derivation, string dossierId)
        {

            var updatedDossier = await _dossiersCollection.UpdateOneAsync(dossier => dossier.Id == dossierId,
                                                           Builders<Dossier>.Update.Push(dossier => dossier.Derivations, derivation));


            if (updatedDossier is null) throw new Exception($"No se pudo actualizar las derivaciones del expediente con identificador {dossierId}");

        }

        private async Task<DossierLastDocumentResponse> GetDossiertLastDocument(string dossierId)
        {
            var dossierLastDocument = await _dossiersCollection.Aggregate<DossierLastDocumentResponse>(GetDossierLastDocumentPipeline(dossierId)).SingleAsync();

            if (dossierLastDocument is null) throw new Exception($"No se pudo obtener el expediente con su último documento con identificador {dossierId}");

            return dossierLastDocument;
        }

        private static BsonDocument[] GetDossierLastDocumentPipeline(string dossierId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(dossierId)));

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var lookUpAggregation = GetDocumentsLookUpPipeline();

            var documentsUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var groupAggregation = MongoDBAggregationExtension.Group(new Dictionary<string, BsonValue>()
            {
                {"_id", "$_id"},
                { "type", MongoDBAggregationExtension.First("$type")  },
                { "client", MongoDBAggregationExtension.First("$client") },
                { "documents", MongoDBAggregationExtension.Push("$documents") }

            });

            var projectAggregation = MongoDBAggregationExtension.Project(new Dictionary<string, BsonValue>()
            {
                {"_id", 1},
                {"type", 1},
                {"client", 1},
                {"documents", 1},
                {"lastDocument", MongoDBAggregationExtension.ArrayElementAt(new List<BsonValue>() { "$documents", -1 }) }
            });

            return new BsonDocument[]
            {
                matchAggregation,
                unwindAggregation,
                lookUpAggregation,
                documentsUnwindAggregation,
                groupAggregation,
                projectAggregation
            };
        }


        private static BsonDocument[] GetDossierGanttDiagramPipeline(string documentNumber)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("client.documentNumber", documentNumber));

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var lookUpAggregation = GetDocumentsLookUpPipeline();

            var documentsUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var projectAggregation = GetGanntDiagramProjectAggregation();

            return new BsonDocument[]
            {
                matchAggregation,
                unwindAggregation,
                lookUpAggregation,
                documentsUnwindAggregation,
                projectAggregation
            };
        }

        private static BsonDocument GetDocumentsLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "documentId", MongoDBAggregationExtension.ObjectId("$documents.documentId") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                        .Eq(new() { "$_id", "$$documentId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documents"));
        }

        private static BsonDocument GetGanntDiagramProjectAggregation()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 1 },
                { "type", "$type" },
                { "documentId", "$documents.documentId" },
                { "documentType", "$documents.type" },
                { "client", "$client" },
                { "creationDate", "$documents.creationDate" },
                { "delayDate", MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.Eq(new() { "$documents.delayDate", BsonNull.Value }), "$documents.excessDate", "$documents.delayDate") },
                { "state", MongoDBAggregationExtension.Switch(new List<BsonValue>()
                {
                   MongoDBAggregationExtension.Case(MongoDBAggregationExtension.In("$documents.state", new BsonArray { "pendiente", "creado", "modificado" }), "pendiente"),
                   MongoDBAggregationExtension.Case(MongoDBAggregationExtension.In("$documents.state", new BsonArray { "procesado", "finalizado", "revisado" }), "procesado"),
                   MongoDBAggregationExtension.Case(MongoDBAggregationExtension.In("$documents.state", new BsonArray { "caducado" }), "caducado")
                }, "") }
            });

            return projectAggregation;
        }

        private static BsonDocument[] GetPaginatedDossiersPipeline(UserDossierPaginationQuery paginationQuery)
        {
            var aggregations = GetDossiersPipeline(paginationQuery).ToList();

            aggregations.Add(MongoDBAggregationExtension.Sort(new BsonDocument("startDate", -1)));
            aggregations.Add(MongoDBAggregationExtension.Skip(paginationQuery.Page * paginationQuery.PageSize));
            aggregations.Add(MongoDBAggregationExtension.Limit(paginationQuery.PageSize));

            return aggregations.ToArray();
        }

        private static BsonDocument[] GetTotalDossiersPipeline(UserDossierPaginationQuery paginationQuery)
        {
            var aggregations = GetDossiersPipeline(paginationQuery);

            var countAggregation = MongoDBAggregationExtension.Count("total");

            return aggregations.Concat(new BsonDocument[] { countAggregation }).ToArray();
        }

        private static BsonDocument[] GetDossiersPipeline(UserDossierPaginationQuery paginationQuery)
        {

            var aggregations = new List<BsonDocument>()
            {
                 GetDossierMatchAggregation(paginationQuery)
            };

            aggregations.AddRange(GetDossierPipelineAggregation(paginationQuery).ToList());
            aggregations.AddRange(DossierListDerivationsPipeline().ToList());
            aggregations.AddRange(DossierListDocumentsPipeline().ToList());
            return aggregations.ToArray();
        }

        private static BsonDocument[] GetDossierPipelineAggregation(UserDossierPaginationQuery paginationQuery)
        {
            var pipelines = new List<BsonDocument>().ToArray();

            var conditions = GetDossiersPipelineConditions();

            conditions.ForEach(condition =>
            {
                if (condition.Condition(paginationQuery)) pipelines = condition.Result(pipelines, paginationQuery);
            });

            return pipelines;
        }

        private static List<FilterConditionDTO<UserDossierPaginationQuery, BsonDocument[]>> GetDossiersPipelineConditions()
        {
            var documentsByUserConditions = new List<FilterConditionDTO<UserDossierPaginationQuery, BsonDocument[]>>()
            {
                new()
                {
                    Condition = (paginationQuery) => !string.IsNullOrEmpty(paginationQuery.ClientName),
                    Result = (documentsByUserPipelines, paginationQuery) =>
                    {
                        var clientPipelines = GetClientSearcherPipeline(paginationQuery.ClientName!);

                        var result = documentsByUserPipelines.Concat(clientPipelines);

                        return result.ToArray();
                    }

                },
                new()
                {
                    Condition = (paginationQuery) => ! string.IsNullOrEmpty(paginationQuery.Type),
                    Result = (documentsByUserPipelines, paginationQuery) =>
                    {
                        var dossierPipelines = GetDossierSearcherPipeline(paginationQuery.Type!);

                        var result = documentsByUserPipelines.Concat(dossierPipelines);

                        return result.ToArray();
                    }
                }
            };

            return documentsByUserConditions;
        }

        private static BsonDocument GetDossierMatchAggregation(UserDossierPaginationQuery paginationQuery)
        {
            var matchedElements = new Dictionary<string, BsonValue>();

            var conditions = GetDocumentsByUserConditions();

            conditions.ForEach(condition =>
            {
                if (condition.Condition(paginationQuery)) matchedElements = condition.Result(matchedElements, paginationQuery);
            });

            var matchAggregation = MongoDBAggregationExtension.Match(matchedElements);

            return matchAggregation;
        }

        private static List<FilterConditionDTO<UserDossierPaginationQuery, Dictionary<string, BsonValue>>> GetDocumentsByUserConditions()
        {
            var documentByUserConditions = new List<FilterConditionDTO<UserDossierPaginationQuery, Dictionary<string, BsonValue>>>()
            {
                new()
                {
                    Condition = (paginationQuery) => !string.IsNullOrEmpty(paginationQuery.Code),
                    Result = (matchedElements, paginationQuery) => {

                        string code = paginationQuery.Code!.Trim();

                        matchedElements.Add("_id", code);

                        return matchedElements;
                    }
                },
                new()
                {
                    Condition = (paginationQuery) => !string.IsNullOrEmpty(paginationQuery.State),
                    Result = (matchedElements, paginationQuery) => {

                        matchedElements.Add("state", paginationQuery.State);

                        return matchedElements;
                    }
                },
                new()
                {
                    Condition = (paginationQuery) => paginationQuery.StartDate.HasValue,
                    Result = (matchedElements, paginationQuery) => {

                        matchedElements.Add("startDate", MongoDBAggregationExtension.GreaterThanEquals(new BsonDateTime(paginationQuery.StartDate!.Value)));

                        return matchedElements;
                    }
                },
                new()
                {
                    Condition = (paginationQuery) => paginationQuery.EndDate.HasValue,
                    Result = (matchedElements, paginationQuery) => {

                        matchedElements.Add("endDate", MongoDBAggregationExtension.LessThanEquals(new BsonDateTime(paginationQuery.EndDate!.Value)));

                        return matchedElements;
                    }
                }
            };

            return documentByUserConditions;
        }

        private static BsonDocument[] GetDossierSearcherPipeline(string dossierType)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("type", dossierType));

            return new BsonDocument[] { matchAggregation };
        }

        private static BsonDocument[] GetClientSearcherPipeline(string clientName)
        {

            var addFieldsAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                { "fullName", MongoDBAggregationExtension.Concat(new List<BsonValue>() { "$client.name", " ", "$client.lastName" }) }
            });

            var matchDictionary = new Dictionary<string, BsonValue>()
            {
                { "fullName", MongoDBAggregationExtension.Regex(clientName.Trim().ToLower() + ".*", "i") }
            };

            var matchAggregation = MongoDBAggregationExtension.Match(matchDictionary);

            var unsetAggregation = MongoDBAggregationExtension.UnSet(new List<BsonValue>() { "fullName" });


            return new BsonDocument[] { addFieldsAggregation, matchAggregation, unsetAggregation };
        }

        private static BsonDocument[] DossierListDerivationsPipeline()
        {
            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$derivations"));

            var lookupAggregation1 = DossierListDerivationsSenderLookUpPipeline();
            var lookupAggregation2 = DossierListDerivationsReceiverLookUpPipeline();
            var lookupAggregation3 = DossierListDerivationsOriginLookUpPipeline();
            var lookupAggregation4 = DossierListDerivationsTargetLookUpPipeline();

            var unwindAggregation2 = MongoDBAggregationExtension.UnWind(new("$senderUser"));
            var unwindAggregation3 = MongoDBAggregationExtension.UnWind(new("$receiverUser"));
            var unwindAggregation4 = MongoDBAggregationExtension.UnWind(new("$originArea"));
            var unwindAggregation5 = MongoDBAggregationExtension.UnWind(new("$targetArea"));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", "$type" },
                { "client", "$client" },
                { "startDate", "$startDate" },
                { "endDate", "$endDate" },
                { "documents", "$documentsHistory" },
                { "derivations.originArea", "$originArea.label"},
                { "derivations.targetArea", "$targetArea.label"},
                { "derivations.senderUser", "$senderUser.name"},
                { "derivations.senderUserProfile", "$senderUser.profile"},
                { "derivations.receiverUser", "$receiverUser.name"},
                { "derivations.receiverUserProfile", "$receiverUser.profile"},
                { "derivations.derivationDate", "$derivations.derivationDate"},
                { "derivations.state", "$derivations.state"},
                { "derivations.type", "$derivations.type"},
                { "state", "$state" }
            });

            var groupAggregation = MongoDBAggregationExtension.Group(new Dictionary<string, BsonValue>()
            {
                { "_id", "$_id"},
                { "type", MongoDBAggregationExtension.First("$type")  },
                { "client", MongoDBAggregationExtension.First("$client") },
                { "startDate", MongoDBAggregationExtension.First("$startDate") },
                { "endDate", MongoDBAggregationExtension.First("$endDate") },
                { "documents", MongoDBAggregationExtension.First("$documents") },
                { "derivations", MongoDBAggregationExtension.Push("$derivations") },
                { "state", MongoDBAggregationExtension.First("$state") }
            });

            return new BsonDocument[] { unwindAggregation, lookupAggregation1, lookupAggregation2, lookupAggregation3, lookupAggregation4, unwindAggregation2, unwindAggregation3, unwindAggregation4, unwindAggregation5, projectAggregation, groupAggregation };
        }
        private static BsonDocument[] DossierListDocumentsPipeline()
        {
            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var lookupAggregation = DossierListDocumentLookUpPipeline();

            var setAggregation = MongoDBAggregationExtension.Set(new Dictionary<string, BsonValue>()
            {
                {"documentInfo", MongoDBAggregationExtension.First("$documentInfo") },
            });

            var addFieldAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                {"documentInfo.client", "$client" },
                {"documentInfo.dossierType" , "$type" }
            });

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "type", 1 },
                { "client", 1 },
                { "startDate", 1 },
                { "endDate", 1 },
                { "documents", "$documentInfo" },
                { "derivations", 1 },
                { "state",  1}
            });

            var groupAggregation = MongoDBAggregationExtension.Group(new Dictionary<string, BsonValue>()
            {
                { "_id", "$_id"},
                { "type", MongoDBAggregationExtension.First("$type")  },
                { "client", MongoDBAggregationExtension.First("$client") },
                { "startDate", MongoDBAggregationExtension.First("$startDate") },
                { "endDate", MongoDBAggregationExtension.First("$endDate") },
                { "documents", MongoDBAggregationExtension.Push("$documents") },
                { "derivations", MongoDBAggregationExtension.First("$derivations") },
                { "state", MongoDBAggregationExtension.First("$state") }
            });

            return new BsonDocument[] { unwindAggregation, lookupAggregation, setAggregation, addFieldAggregation, projectAggregation, groupAggregation };
        }

        private static BsonDocument DossierListDerivationsSenderLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userObjId", MongoDBAggregationExtension.ObjectId("$derivations.senderUser") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userObjId" }))),
                MongoDBAggregationExtension.Project(new(){
                    { "_id", 0 },
                    { "name", MongoDBAggregationExtension.Concat(new List<BsonValue>(){ "$data.name", " ","$data.lastName"})},
                    { "profile", "$data.profile" }
                })
            };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "senderUser"));
        }

        private static BsonDocument DossierListDocumentLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userObjId", MongoDBAggregationExtension.ObjectId("$documents.documentId") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userObjId" }))),
                MongoDBAggregationExtension.Project(new(){
                    { "type", 1 },
                    {"contentsHistory", 1 },
                    {"processesHistory", 1 },
                    { "attachedUrls", 1 },
                    {"state", 1 },
                    { "creationDate", 1 },
                    {"content", 1}
            })
        };

            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documentInfo"));
        }

        private static BsonDocument DossierListDerivationsReceiverLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userObjId", MongoDBAggregationExtension.ObjectId("$derivations.receiverUser") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userObjId" }))),
                MongoDBAggregationExtension.Project(new(){
                    { "_id", 0 },
                    { "name", MongoDBAggregationExtension.Concat(new List<BsonValue>(){ "$data.name", " ","$data.lastName"})},
                    {"profile", "$data.profile" }
            })
        };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "receiverUser"));
        }

        private static BsonDocument DossierListDerivationsOriginLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userObjId", MongoDBAggregationExtension.ObjectId("$derivations.originArea") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userObjId" }))),
                MongoDBAggregationExtension.Project(new(){
                { "_id", 0 },
                { "label", 1}
            })
        };

            return MongoDBAggregationExtension.Lookup(new("roles", letPipeline, lookUpPipeline, "originArea"));
        }

        private static BsonDocument DossierListDerivationsTargetLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userObjId", MongoDBAggregationExtension.ObjectId("$derivations.targetArea") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userObjId" }))),
                MongoDBAggregationExtension.Project(new(){
                { "_id", 0 },
                { "label", 1}
            })
        };

            return MongoDBAggregationExtension.Lookup(new("roles", letPipeline, lookUpPipeline, "targetArea"));
        }

        private BsonDocument[] GetDossierBydDocumentIdPipeline(string documentId)
        {
            var lookupAggregation = GetDocumentLastPublicDeedLookUpPipeline();
            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$dossiers"));
            var projectAggregation = MongoDBAggregationExtension.Project(new()
                {
                    { "_id", 0 },
                    {"dossier", "$dossiers" }
                });
            var replaceRootAggregation = MongoDBAggregationExtension.ReplaceRoot("$dossier");

            return new BsonDocument[] { lookupAggregation, unWindAggregation, projectAggregation, replaceRootAggregation };
        }

        private static BsonDocument GetDossierBydDocumentIdLookupPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userObjId", MongoDBAggregationExtension.ToString("$_id") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { MongoDBAggregationExtension.In("$$userObjId", "$documentsHistory.documentId"), true })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "dossiers"));
        }
        #endregion
    }
}
