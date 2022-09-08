using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Requests.Dossier;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Repositories
{
    public class DossierService : IDossierService
    {
        private readonly IMongoCollection<Dossier> _dossiersCollection;

        public DossierService(IMongoDatabase mongoDatabase)
        {
            _dossiersCollection = mongoDatabase.GetCollection<Dossier>(CollectionName);
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
            //TODO: Implement the register of the document into the input and ouput tray of sent user in the method
            return await GetDossiertLastDocument(dossierLastDocumentRequest.Id);
        }

        #region private methods
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
