using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentProcess;

namespace SISGED.Server.Services.Repositories
{
    public class DocumentProcessService : IDocumentProcessService
    {
        private readonly IMongoCollection<Document> _documentsCollection;

        public string CollectionName => "documentos";

        public DocumentProcessService(IMongoDatabase mongoDatabase)
        {
            _documentsCollection = mongoDatabase.GetCollection<Document>(CollectionName);
        }

        public async Task<IEnumerable<DocumentProcessInfo>> GetProcessesByDocumentIdAsync(string documentId)
        {
            var processes = await _documentsCollection.Aggregate<DocumentProcessInfo>(GetProcessesByDocumentIdPipeline(documentId)).ToListAsync();

            if (processes is null) throw new Exception($"No se pudo encontrar los procesos realizadas del document con identificador {documentId}");

            return processes;
        }

        private static BsonDocument[] GetProcessesByDocumentIdPipeline(string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("_id", new ObjectId(documentId)));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "processesHistory", 1  }
            });

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$processesHistory"));

            var senderUserLookUpAggregation = GetSenderUserLookUpPipeline();

            var senderUserUnWindAggregation = MongoDBAggregationExtension.UnWind(new("$senderUser"));

            var receiverUserLookUpAggregation = GetReceiverUserLookUpPipeline();

            var receiverUserUnWindAggregation = MongoDBAggregationExtension.UnWind(new("$receiverUser"));

            var roleLookUpAggregation = GetRoleLookUpPipeline();

            var roleUnWindAggregation = MongoDBAggregationExtension.UnWind(new("$role"));

            var processProjectAggregation = GetProcessProjectPipeline();

            var unsetAggregation = MongoDBAggregationExtension.UnSet("_id");

            return new BsonDocument[] { matchAggregation, projectAggregation, unwindAggregation, senderUserLookUpAggregation, senderUserUnWindAggregation,
               receiverUserLookUpAggregation, receiverUserUnWindAggregation, roleLookUpAggregation, roleUnWindAggregation, processProjectAggregation,
               unsetAggregation };
        }

        private static BsonDocument GetProcessProjectPipeline()
        {
            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "area", "$role.label"  },
                { "receiptDate", "$processesHistory.receiptDate" },
                { "state", "$processesHistory.state" },
                { "senderUser",  new BsonDocument()
                                .Add("userId", "$senderUser._id")
                                .Add("firstName", "$senderUser.data.name")
                                .Add("lastName", "$senderUser.data.lastName")
                                .Add("image", "$senderUser.data.profile")
                },
                { "receiverUser", new BsonDocument()
                                .Add("userId", "$receiverUser._id")
                                .Add("firstName", "$receiverUser.data.name")
                                .Add("lastName", "$receiverUser.data.lastName")
                                .Add("image", "$receiverUser.data.profile")
                }
            });

            return projectAggregation;
        }

        private static BsonDocument GetRoleLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "roleId", MongoDBAggregationExtension.ObjectId("$processesHistory.area") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$roleId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("roles", letPipeline, lookUpPipeline, "role"));
        }

        private static BsonDocument GetSenderUserLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userId", MongoDBAggregationExtension.ObjectId("$processesHistory.senderId") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userId"})))
            };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "senderUser"));
        }

        private static BsonDocument GetReceiverUserLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userId", MongoDBAggregationExtension.ObjectId("$processesHistory.receiverId") }
            };

            var lookUpPipeline = new BsonArray()
            {
                MongoDBAggregationExtension.Match(
                    MongoDBAggregationExtension.Expr(MongoDBAggregationExtension.Eq(new() { "$_id", "$$userId" })))
            };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "receiverUser"));
        }
    }
}
