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
                { "historialproceso", 1  }
            });

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$historialproceso"));

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
                { "receiptDate", "$historialproceso.fechaemision" },
                { "state", "$historialproceso.estado" },
                { "senderUser",  new BsonDocument()
                                .Add("userId", "$senderUser._id")
                                .Add("firstName", "$senderUser.datos.nombre")
                                .Add("lastName", "$senderUser.datos.apellido")
                                .Add("image", "$senderUser.datos.imagen")
                },
                { "receiverUser", new BsonDocument()
                                .Add("userId", "$receiverUser._id")
                                .Add("firstName", "$receiverUser.datos.nombre")
                                .Add("lastName", "$receiverUser.datos.apellido")
                                .Add("image", "$receiverUser.datos.imagen")
                }
            });

            return projectAggregation;
        }

        private static BsonDocument GetRoleLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "roleId", MongoDBAggregationExtension.ObjectId("$historialproceso.area") }
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
                { "userId", MongoDBAggregationExtension.ObjectId("$historialproceso.idemisor") }
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
                { "userId", MongoDBAggregationExtension.ObjectId("$historialproceso.idreceptor") }
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
