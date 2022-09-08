using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Server.Services.Repositories
{
    public class TrayService : ITrayService
    {
        private readonly IMongoCollection<Tray> _traysCollection;
        private readonly IMongoCollection<User> _usersCollection;
        public string CollectionName => "bandejas";
        public string SecondCollectionName => "usuarios";

        public TrayService(IMongoDatabase mongoDatabase)
        {
            _traysCollection = mongoDatabase.GetCollection<Tray>(CollectionName);
            _usersCollection = mongoDatabase.GetCollection<User>(SecondCollectionName);
        }

        public async Task RegisterUserTrayAsync(string type, string userId)
        {
            Tray userTray = new(type, userId);

            await _traysCollection.InsertOneAsync(userTray);

            if (string.IsNullOrEmpty(userTray.Id)) throw new Exception($"No se pudo registrar la bandeja del usuario {userId} al sistema");
        }

        public async Task<InputOutputTrayResponse> GetAsync(string user)
        {
            var tray = await _traysCollection.Aggregate<InputOutputTrayResponse>(GetTrayPipeline(user)).FirstAsync();

            if (tray is null) throw new Exception("No se ha podido encontrar la bandeja.");

            return tray;

        }

        public async Task<InputTrayResponse> GetInputStrayAsync(string user)
        {
            var tray = await _traysCollection.Aggregate<InputTrayResponse>(GetInputTrayPipeline(user)).FirstAsync();

            if (tray is null) throw new Exception("No se ha podido encontrar la bandeja.");

            return tray;
        }

        public async Task<Tray> GetTrayDocumentAsync(string user)
        {
            return await _traysCollection.FindAsync(t => t.User == user).Result.FirstAsync();
        }

        #region private methods
        private PipelineDefinition<Tray, InputTrayResponse> GetInputTrayPipeline(string user)
        {
            var dossierInputLookupPipeline = DossierInputLookUpPipeline();

            var conditionalFilter = MongoDBAggregationExtension.Eq(new BsonArray() { "$$item.iddocumento", MongoDBAggregationExtension.ObjectId("$bandejaentrada.iddocumento") });

            var filter = MongoDBAggregationExtension.Filter("$bandejadocumento.documentos", conditionalFilter);

            var varDeclaration = new BsonDocument("$let", new BsonDocument("vars", new BsonDocument("documento", filter))
                                                            .AddRange(MongoDBAggregationExtension.In("$arrayElemAt", new BsonArray { "$$documento", 0 })));

            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "Document", varDeclaration },
                { "InputTray", 1 },
                { "DossierType", "$bandejadocumento.tipo" },
                { "Client", "$bandejadocumento.cliente" }
            });

            var userFilter = Builders<Tray>.Filter.Eq("usuario", user).ToBsonDocument();
            var userFilterMatchAggregation = MongoDBAggregationExtension.Match(userFilter);

            var unWindAggregation1 = MongoDBAggregationExtension.UnWind(new("bandejaentrada"));
            var unWindAggregation2 = MongoDBAggregationExtension.UnWind(new("bandejadocumento"));

            return new BsonDocument[] { userFilterMatchAggregation, unWindAggregation1, dossierInputLookupPipeline, unWindAggregation2, projectAggregation };

        }
        private PipelineDefinition<Tray, InputOutputTrayResponse> GetTrayPipeline(string user)
        {
            var dossierLookupPipeline = DossierLookUpPipeline();
            var documentLookupPipeline = DocumentLookUpPipeline();

            /* Group para agrupar los documentos en un array */

            var groupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$bandejadocumento._id" },
                { "cliente", MongoDBAggregationExtension.First("$bandejadocumento.cliente") },
                { "tipo", MongoDBAggregationExtension.First("$bandejadocumento.tipo") },
                { "documentosobj", MongoDBAggregationExtension.Push("$documentosobj") },
                { "bandejasalida", MongoDBAggregationExtension.First("$bandejasalida") },
                { "bandejaentrada", MongoDBAggregationExtension.First("$bandejaentrada") },
                { "idbandeja", MongoDBAggregationExtension.First("$_id") }
            });

            /* Proyección para devolver los datos que se necesitan */

            var conditionalFilter = MongoDBAggregationExtension.Eq(new BsonArray() { "$$item._id", MongoDBAggregationExtension.ObjectId("$bandejasalida.iddocumento") });

            var filtro = MongoDBAggregationExtension.Filter("$documentosobj", conditionalFilter);

            var declararVariable = new BsonDocument("$let", new BsonDocument("vars", new BsonDocument("documento", filtro))
                                                            .AddRange(MongoDBAggregationExtension.In("$arrayElemAt", new BsonArray { "$$documento", 0 })));

            var documentProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "documento", declararVariable },
                { "documentosobj", "$documentosobj" },
                { "tipo", "$tipo" },
                { "_id", "$_id" },
                { "cliente", "$cliente" },
                { "bandejaentrada", 1 },
                { "idbandeja", 1 }
            });

            var outputDossierProjectAux = new BsonDocument
            {
                { "idexpediente", "$_id" },
                { "cliente", "$cliente" },
                { "tipo", "$tipo" },
                { "documentosobj", "$documentosobj" },
                { "documento", "$documento" }
            };
            /* Proyección para crear el atributo de expediente salida*/
            var projectOutputCreationAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "idbandeja", 1 },
                { "bandejaentrada", 1 },
                { "expedientesalida", outputDossierProjectAux }
            });

            /* Group para unificar todos los expedientes de la bandeja de salida*/
            var groupOutputTrayDossierAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$idbandeja" },
                { "bandejaentrada", MongoDBAggregationExtension.First("$bandejaentrada") },
                { "expedientesalida", MongoDBAggregationExtension.Push("$expedientesalida") }
            });

            /* Lookup para los expedientes de la bandeja de entrada*/
            var dossierInputTrayLookUpAggregation = DossierInputTrayLookUpPipeline();

            /* Group para la bandeja de entrada y agrupos los elementos en un array*/
            var inputGroupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$bandejadocumento._id" },
                { "cliente", MongoDBAggregationExtension.First("$bandejadocumento.cliente") },
                { "tipo", MongoDBAggregationExtension.First("$bandejadocumento.tipo") },
                { "documentosobj", MongoDBAggregationExtension.Push("$documentosobj") },
                { "expedientesalida", MongoDBAggregationExtension.First("$expedientesalida") },
                { "bandejaentrada", MongoDBAggregationExtension.First("$bandejaentrada") },
                { "idbandeja", MongoDBAggregationExtension.First("$_id") }
            });

            /* Proyección para encontrar el documento de entrada */

            var conditionalInputFilter = MongoDBAggregationExtension.Eq(new BsonArray { "$$item._id", MongoDBAggregationExtension.ObjectId("$bandejaentrada.iddocumento") });

            var inputfilter = MongoDBAggregationExtension.Filter("$documentosobj", conditionalInputFilter);

            var inputVarDeclaration = new BsonDocument("$let", new BsonDocument("vars", new BsonDocument("documento", inputfilter))
                                                            .AddRange(MongoDBAggregationExtension.In("$arrayElemAt", new BsonArray { "$$documento", 0 })));

            var inputProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "documento", inputVarDeclaration },
                { "documentosobj", "$documentosobj" },
                { "tipo", "$tipo" },
                { "_id", "$_id" },
                { "cliente", "$cliente" },
                { "bandejaentrada", 1 },
                { "idbandeja", 1 },
                { "expedientesalida", 1 }
            });

            var inputDossierProjectBsonAux = new BsonDocument()
            {
                { "DossierId", "$_id" },
                { "Client", "$cliente" },
                { "Type", "$tipo" },
                { "DocumentObjects", "$documentosobj" },
                { "Document", "$documento" }
            };

            /* Proyección para crear el expedienteentrada*/
            var inputDossierProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "Id", 1},
                { "OutputDossier", 1 },
                { "InputDossier", inputDossierProjectBsonAux }
            });

            /* Group final para la unión de los expedientes */
            var groupFinalAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$idbandeja" },
                { "InputDossier", MongoDBAggregationExtension.First("$InputDossier") },
                { "OutputDossier", MongoDBAggregationExtension.Push("$OutputDossier") },
            });

            var userfilter = Builders<Tray>.Filter.Eq("usuario", user).ToBsonDocument();

            var userFilterMatchAggregation = MongoDBAggregationExtension.Match(userfilter);

            var unWindAggregation0 = MongoDBAggregationExtension.UnWind(new("$documentosobj"));
            var unWindAggregation1 = MongoDBAggregationExtension.UnWind(new("$bandejasalida"));
            var unWindAggregation2 = MongoDBAggregationExtension.UnWind(new("$bandejadocumento"));
            var unWindAggregation3 = MongoDBAggregationExtension.UnWind(new("$bandejadocumento.Documents"));
            var unWindAggregation4 = MongoDBAggregationExtension.UnWind(new("$documentosobj"));
            var unWindAggregation5 = MongoDBAggregationExtension.UnWind(new("$bandejaentrada"));
            var unWindAggregation6 = MongoDBAggregationExtension.UnWind(new("$bandejadocumento"));
            var unWindAggregation7 = MongoDBAggregationExtension.UnWind(new("$bandejadocumento.Documents"));

            return new BsonDocument[] { userFilterMatchAggregation, unWindAggregation0, dossierLookupPipeline, unWindAggregation1, unWindAggregation2, documentLookupPipeline, unWindAggregation3, groupAggregation, documentProjectAggregation, projectOutputCreationAggregation, groupOutputTrayDossierAggregation, unWindAggregation4, dossierInputTrayLookUpAggregation, unWindAggregation5, unWindAggregation6, documentLookupPipeline, unWindAggregation7, inputGroupAggregation, inputProjectAggregation, inputDossierProjectAggregation, groupFinalAggregation };
        }
        private static BsonDocument DossierInputTrayLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "expedienteId", "$bandejaentrada.idexpediente" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$expedienteId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "bandejadocumento"));
        }

        private static BsonDocument DossierLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "expedienteId", "$bandejasalida.idexpediente" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$expedienteId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "bandejadocumento"));
        }

        private static BsonDocument DocumentLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "documentoId", "$bandejadocumento.documentos.iddocumento" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$documentoId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documentosobj"));
        }
        private static BsonDocument DossierInputLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "expedienteId", "$bandejaentrada.idexpediente" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$expedienteId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "bandejadocumento"));
        }
        #endregion
    }
}
