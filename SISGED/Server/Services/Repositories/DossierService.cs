using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Dossier;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Repositories
{
    public class DossierService : IDossierService
    {
        private readonly IMongoCollection<Dossier> _dossiersCollection;
        private readonly ITrayService _trayService;
        private readonly IDocumentService _documentService;

        public DossierService(IMongoDatabase mongoDatabase, ITrayService trayService, IDocumentService documentService)
        { 

            _dossiersCollection = mongoDatabase.GetCollection<Dossier>(CollectionName);
            _trayService = trayService;
            _documentService = documentService;
        }

        public string CollectionName => "expedientes";

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
            Derivation dossierDerivation = dossierLastDocumentRequest.Derivations.FirstOrDefault()!;

            dossierDerivation.ReceiverUser = userId;

            await UpdateDossierDerivationsAsync(dossierDerivation, userId);

            string documentId = dossierLastDocumentRequest.Documents.Last().DocumentId;

            var updateTrayDTO = new UpdateTrayDTO(dossierLastDocumentRequest.Id, userId, 
                                                  dossierDerivation.SenderUser,
                                                  documentId);
            
            var usersTraysupdate = _trayService.UpdateTrayForDerivationAsync(updateTrayDTO);

            var process = new Process(dossierDerivation.OriginArea, dossierDerivation.SenderUser, dossierDerivation.ReceiverUser);

            var documentProcessupdate = _documentService.UpdateDocumentProcessAsync(process, documentId);

            await Task.WhenAll(usersTraysupdate, documentProcessupdate);

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
                                                 .Push(dossierToUpdate => dossierToUpdate.Documents, dossier.Documents.First());

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

        #region private methods
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
                                           dossiersFilter.Add("estado", MongoDBAggregationExtension.Regex(dossierHistoryQuery.State! + ".*", "i"))
                },
                new()
                {
                    Condition = (DossierHistoryQuery dossierHistoryQuery) => !string.IsNullOrEmpty(dossierHistoryQuery.Type),
                    Result = (BsonDocument dossiersFilter, DossierHistoryQuery dossierHistoryQuery) => 
                                            dossiersFilter.Add("tipo", MongoDBAggregationExtension.Regex(dossierHistoryQuery.Type! + ".*", "i"))
                },
                new()
                {
                    Condition = (DossierHistoryQuery dossierHistoryQuery) => !string.IsNullOrEmpty(dossierHistoryQuery.ClientName),
                    Result = (BsonDocument dossiersFilter, DossierHistoryQuery dossierHistoryQuery) => 
                                           dossiersFilter.Add("cliente.nombre", MongoDBAggregationExtension.Regex(dossierHistoryQuery.ClientName! + ".*", "i"))
                }
            };

            return dossierConditions;
        }

        private async Task UpdateDossierDerivationsAsync(Derivation derivation, string dossierId)
        {

            var updatedDossier = await _dossiersCollection.UpdateOneAsync(dossier => dossier.Id == dossierId, 
                                                           Builders<Dossier>.Update.Push(dossier => dossier.Derivations, derivation));


            if (updatedDossier is null) throw new Exception($"No se pudo actualizar las derivaciones del expediente con identificador { dossierId }");

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

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$documentos"));

            var lookUpAggregation = GetDocumentsLookUpPipeline();

            var documentsUnwindAggregation = MongoDBAggregationExtension.UnWind(new("$documents"));

            var groupAggregation = MongoDBAggregationExtension.Group(new Dictionary<string, BsonValue>()
            {
                {"_id", "$_id"},
                { "type", MongoDBAggregationExtension.First("$tipo")  },
                { "client", MongoDBAggregationExtension.First("$cliente") },
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
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("cliente.numerodocumento", documentNumber));

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$documentos"));

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
                { "documentId", MongoDBAggregationExtension.ObjectId("$documentos.iddocumento") }
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
                { "type", "$tipo" },
                { "documentId", "$documentos.iddocumento" },
                { "documentType", "$documentos.tipo" },
                { "client", "$cliente" },
                { "creationDate", "$documentos.fechacreacion" },
                { "delayDate", MongoDBAggregationExtension.Cond(MongoDBAggregationExtension.Eq(new() { "$documentos.fechademora", BsonNull.Value }), "$documentos.fechaexceso", "$documentos.fechademora") },
                { "state", MongoDBAggregationExtension.Switch(new List<BsonValue>()
                {
                   MongoDBAggregationExtension.Case(MongoDBAggregationExtension.In("$documents.estado", new BsonArray { "pendiente", "creado", "modificado" }), "pendiente"),
                   MongoDBAggregationExtension.Case(MongoDBAggregationExtension.In("$documents.estado", new BsonArray { "procesado", "finalizado", "revisado" }), "procesado"),
                   MongoDBAggregationExtension.Case(MongoDBAggregationExtension.In("$documents.estado", new BsonArray { "caducado" }), "caducado")
                }, "") }
            });

            return projectAggregation;
        }

        #endregion
    }
}
