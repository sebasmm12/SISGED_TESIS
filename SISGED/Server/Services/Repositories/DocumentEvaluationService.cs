using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Models.Responses.DocumentEvaluation;
using System.Reflection.Metadata;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentEvaluationService : IDocumentEvaluationService
    {
        private readonly IMongoCollection<Document> _documentsCollection;

        public string CollectionName => "documentos";

        public DocumentEvaluationService(IMongoDatabase mongoDatabase)
        {
            _documentsCollection = mongoDatabase.GetCollection<Document>(CollectionName);
        }

        public async Task<IEnumerable<DocumentEvaluationInfo>> GetEvaluationsByDocumentIdAsync(string documentId)
        {
            var evaluations = await _documentsCollection.Aggregate<DocumentEvaluationInfo>(GetEvaluationsByDocumentIdPipeline(documentId)).ToListAsync();

            if (evaluations is null) throw new Exception($"No se pudo obtener las evaluaciones del document con identificador { documentId }");

            return evaluations;
        }

        private static BsonDocument[] GetEvaluationsByDocumentIdPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "evaluations", 1  }
            });

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$evaluations"));

            var evaluatorUserLookUpAggregation = GetEvaluatorUserLookUpPipeline();

            var evaluatorUserUnWindAggregation = MongoDBAggregationExtension.UnWind(new("$evaluatorUser"));

            var processProjectAggregation = GetEvaluationProjectPipeline();

            var unsetAggregation = MongoDBAggregationExtension.UnSet("_id");

            return new BsonDocument[] { matchAggregation, projectAggregation, unwindAggregation, evaluatorUserLookUpAggregation, evaluatorUserUnWindAggregation,
                    processProjectAggregation, unsetAggregation };

        }

        private static BsonDocument GetEvaluationProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "isApproved", "$evaluations.isApproved" },
                { "comment", "$evaluations.comment" },
                { "evaluationDate", "$evaluations.evaluationDate"  },
                { "evaluatorUser",  new BsonDocument()
                                .Add("firstName", "$evaluatorUser.data.name")
                                .Add("lastName", "$evaluatorUser.data.lastName")
                                .Add("image", "$evaluatorUser.data.profile")
                },
            });

            return projectAggregation;
        }

        private static BsonDocument GetEvaluatorUserLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userId", MongoDBAggregationExtension.ObjectId("$evaluations.userEvaluator") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userId"})))
            };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "evaluatorUser"));
        }
    }
}
