using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.Statistic;
using SISGED.Shared.Models.Responses.Statistic;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentService : IDocumentService
    {
        private readonly IMongoCollection<Document> _documentsCollection;
        public string CollectionName => "documentos";
        public DocumentService(IMongoDatabase mongoDatabase)
        {
            _documentsCollection = mongoDatabase.GetCollection<Document>(CollectionName);
        }

        public async Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAndAreaAsync(DocumentsByMonthAndAreaQuery documentsByMonthAndAreaQuery)
        {
            var documentsByMonthAndArea = await _documentsCollection.Aggregate<DocumentsByMonthAndAreaResponse>(GetDocumentsByMonthAndAreaPipeline(documentsByMonthAndAreaQuery)).ToListAsync();

            if (documentsByMonthAndArea is null) throw new Exception($"No se pudo obtener los documentos en el { documentsByMonthAndAreaQuery.Month } mes y en el área { documentsByMonthAndAreaQuery.Area }");

            return documentsByMonthAndArea;
        }

        public async Task<IEnumerable<DocumentsByMonthAndAreaResponse>> GetDocumentsByMonthAsync(DocumentsByMonthQuery documentsByMonthQuery)
        {
            var documentsByMonth = await _documentsCollection.Aggregate<DocumentsByMonthAndAreaResponse>(GetDocumentsByMonthPipeline(documentsByMonthQuery)).ToListAsync();

            if (documentsByMonth is null) throw new Exception($"No se pudo obtener los documentos en el { documentsByMonthQuery.Month } mes");

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

            if (documentsByState is null) throw new Exception($"No se pudo obtener los documentos por estado en el {documentsByStateQuery.Month } mes");

            return documentsByState;
        }

        public async Task UpdateDocumentProcessAsync(Process proccess, string documentId)
        {
            var updateDocumentProccess = Builders<Document>.Update.Push(document => document.ProcessesHistory, proccess);

            var updatedDocument = await _documentsCollection.UpdateOneAsync(document => document.Id == documentId, updateDocumentProccess);

            if (updatedDocument is null) throw new Exception($"No se pudo actualizar el historial del proceso del documento con identificador { documentId }");
        }

        #region private methods
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

        // TODO: Create a method with all query inputs and validate if anyone of them is null and if this values is null, it will have to create a diferente bsonDocument Filter.

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
