using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Server.Services.Repositories
{
    public class TrayService : ITrayService
    {
        private readonly IMongoCollection<Tray> _traysCollection;

        public string CollectionName => "bandejas";
        public string SecondCollectionName => "usuarios";

        public TrayService(IMongoDatabase mongoDatabase)
        {
            _traysCollection = mongoDatabase.GetCollection<Tray>(CollectionName);
        }

        public async Task MoveUserOutPutToInputTrayAsync(UserTrayAnnulmentDTO userTrayAnnulmentDTO)
        {
            var currentDocumentTray = new DocumentTray(userTrayAnnulmentDTO.DossierId, userTrayAnnulmentDTO.CurrentDocumentId);
            var newDocumentTray = new DocumentTray(userTrayAnnulmentDTO.DossierId, userTrayAnnulmentDTO.NewDocumentId);

            var outputTrayUpdate = PullDocumentTrayAsync(new(currentDocumentTray, userTrayAnnulmentDTO.CurrentUserId, "outputTray"));
            
            var inputTrayUpdate = PushDocumentTrayAsync(new(newDocumentTray, userTrayAnnulmentDTO.CurrentUserId, "inputTray"));

            await Task.WhenAll(inputTrayUpdate, outputTrayUpdate);
        }

        public async Task MoveUserTrayAsync(UserTrayAnnulmentDTO userTrayAnnulmentDTO)
        {
            var currentDocumentTray = new DocumentTray(userTrayAnnulmentDTO.DossierId, userTrayAnnulmentDTO.CurrentDocumentId);
            var newDocumentTray = new DocumentTray(userTrayAnnulmentDTO.DossierId, userTrayAnnulmentDTO.NewDocumentId);

            var currentUserinputTrayUpdate = PullDocumentTrayAsync(new(currentDocumentTray, userTrayAnnulmentDTO.CurrentUserId, "inputTray"));

            var newUserinputTrayUpdate = PushDocumentTrayAsync(new(newDocumentTray, userTrayAnnulmentDTO.NewUserId, "inputTray"));

            await Task.WhenAll(currentUserinputTrayUpdate, newUserinputTrayUpdate);
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

            tray.InputDossier = tray.InputDossier.Where(inputDossier => !string.IsNullOrEmpty(inputDossier.DossierId)).ToList();
            tray.OutputDossier = tray.OutputDossier.Where(ouputDossier => !string.IsNullOrEmpty(ouputDossier.DossierId)).ToList();

            return tray;

        }

        public async Task<InputTrayResponse> GetInputStrayAsync(string user)
        {
            var tray = await _traysCollection.Aggregate(GetInputTrayPipeline(user)).FirstAsync();

            if (tray is null) throw new Exception("No se ha podido encontrar la bandeja.");

            return tray;
        }

        public async Task<DocumentTray> GetDocumentTrayByUserIdDocumentIdAsync(string userId, string documentId)
        {
            var doc = await _traysCollection.Aggregate<DocumentTray>(GetTrayByUserIdDocumentIdPipeline(userId, documentId)).FirstOrDefaultAsync();

            if (doc is null) throw new Exception("No se ha podido encontrar el documento en la bandeja.");

            return doc;
        }

        public async Task<Tray> GetTrayDocumentAsync(string user)
        {
            return await _traysCollection.FindAsync(t => t.User == user).Result.FirstAsync();
        }

        public async Task<IEnumerable<UserTrayResponse>> GetWorkloadByRoleAsync(string role)
        {
            var tray = await _traysCollection.Aggregate<UserTrayResponse>(GetWorkloadByRolePipeline(role)).ToListAsync();

            if (tray is null) throw new Exception("No se ha podido encontrar la bandeja.");

            return tray;
        }

        public async Task UpdateTrayForDerivationAsync(UpdateTrayDTO updateTrayDTO)
        {
            var documentTray = new DocumentTray(updateTrayDTO.DossierId, updateTrayDTO.DocumentId);

            var receiverInputTrayUpdate = PushDocumentTrayAsync(new(documentTray, updateTrayDTO.RecieverUserId, "inputTray"));

            var senderOutputTrayUpdate = PullDocumentTrayAsync(new(documentTray, updateTrayDTO.SenderUserId, "outputTray"));

            var senderInputTrayUpdate = PullDocumentTrayAsync(new(documentTray, updateTrayDTO.SenderUserId, "inputTray"));

            await Task.WhenAll(receiverInputTrayUpdate, senderOutputTrayUpdate, senderInputTrayUpdate);
        }

        public async Task RegisterOutputTrayAsync(OutPutTrayDTO outPutTrayDTO)
        {
            var currentDocumentTray = new DocumentTray(outPutTrayDTO.DossierId, outPutTrayDTO.CurrentDocumentId);
            var newDocumentTray = new DocumentTray(outPutTrayDTO.DossierId, outPutTrayDTO.NewDocumentId);

            var inputTrayUpdate = PullDocumentTrayAsync(new(currentDocumentTray, outPutTrayDTO.UserId, "inputTray"));

            var outputTrayUpdate = PushDocumentTrayAsync(new(newDocumentTray, outPutTrayDTO.UserId, "outputTray"));

            await Task.WhenAll(inputTrayUpdate, outputTrayUpdate);
        }
        
        public async Task RegisterOutputTrayWithDocumentTrayAsync(DocumentTray document, User user) { 

            var inputTrayUpdate = PullDocumentTrayAsync(new(document, user.Id, "inputTray"));

            var outputTrayUpdate = PushDocumentTrayAsync(new(document, user.Id, "outputTray"));

            await Task.WhenAll(inputTrayUpdate, outputTrayUpdate);
        }

        public async Task<string> RegisterUserInputTrayAsync(string dossierId, string documentId, string type)
        {
            var documentTray = new DocumentTray(dossierId, documentId);

            Tray userTray = await GetUserTrayWithLessInputTrayAsync(type);

            await PushDocumentTrayAsync(new(documentTray, userTray.User, "inputTray"));

            return userTray.User;
        }

        public async Task<Tray> DeleteInputTrayDocumentAsync(string documentId)
        {
            var currentTray = await GetTrayByDocumentAsync(documentId);

            var trayDocument = currentTray.InputTray.First(trayDocument => trayDocument.DocumentId == documentId);

            var trayUpdate = Builders<Tray>.Update.Pull("inputTray", trayDocument);

            var updatedTray = await _traysCollection.FindOneAndUpdateAsync(tray => tray.Id == currentTray.Id, trayUpdate, new()
            {
                ReturnDocument = ReturnDocument.After
            });

            if (updatedTray is null) throw new Exception($"No se pudo eliminar el documento con identificador {documentId} de la bandeja con identificador {currentTray.Id}");

            return updatedTray;
        }

        public async Task PushDocumentTrayAsync(UpdateDocumentTrayDTO updateDocumentTrayDTO)
        {
            var updateDocumentTray = Builders<Tray>.Update.Push(updateDocumentTrayDTO.TrayType, updateDocumentTrayDTO.DocumentTray);

            var updateTray = await _traysCollection.UpdateOneAsync(tray => tray.User == updateDocumentTrayDTO.UserId, updateDocumentTray);

            if (updateTray is null) throw new Exception($"No se pudo actualizar la bandeja del usuario con identificador {updateDocumentTrayDTO.UserId}");
        }

        public async Task PullDocumentTrayAsync(UpdateDocumentTrayDTO updateDocumentTrayDTO)
        {
            var updateDocumentTray = Builders<Tray>.Update.Pull(updateDocumentTrayDTO.TrayType, updateDocumentTrayDTO.DocumentTray);

            var updateTray = await _traysCollection.UpdateOneAsync(tray => tray.User == updateDocumentTrayDTO.UserId, updateDocumentTray);

            if (updateTray is null) throw new Exception($"No se pudo actualizar la bandeja del usuario con identificador {updateDocumentTrayDTO.UserId}");
        }

        #region private methods
        private async Task<Tray> GetTrayByDocumentAsync(string documentId)
        {
            var trayDocumentBuilder = Builders<DocumentTray>.Filter.Eq(dosssierDocument => dosssierDocument.DocumentId, documentId);

            var trayBuilder = Builders<Tray>.Filter.ElemMatch(tray => tray.InputTray, trayDocumentBuilder);

            var tray = await _traysCollection.Find(trayBuilder).FirstOrDefaultAsync();

            if (tray is null) throw new Exception($"No se pudo encontrar el expediente que tiene el document con identificador {documentId} relacionado");

            return tray;
        }
        private async Task<Tray> GetUserTrayWithLessInputTrayAsync(string type)
        {
            var userTray = await _traysCollection.Aggregate<Tray>(GetUserTrayWithLessInputTrayPipeLine(type)).FirstOrDefaultAsync();

            if (userTray is null) throw new Exception($"No se pudo encontrar el usuario con bandeja mediante el tipo {type}");

            return userTray;
        }

        private static BsonDocument[] GetUsersTraysWithLessInputTray(string type)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("type", type));

            var addFieldAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                { "totalTrays", MongoDBAggregationExtension.Add(new List<BsonValue>() { MongoDBAggregationExtension.Size("$inputTray"), MongoDBAggregationExtension.Size("$outputTray")  })   }
            });

            var sortAggregation = MongoDBAggregationExtension.Sort(new BsonDocument("totalTrays", 1));

            var unSetAggregation = MongoDBAggregationExtension.UnSet("totalTrays");

            return new BsonDocument[] { matchAggregation, addFieldAggregation, sortAggregation, unSetAggregation };
        }

        private static BsonDocument[] GetUserTrayWithLessInputTrayPipeLine(string type)
        {
            var usersTraysWithLessInputTray = GetUsersTraysWithLessInputTray(type);

            var limitAggregation = MongoDBAggregationExtension.Limit(1);

            return usersTraysWithLessInputTray.Append(limitAggregation).ToArray();
        }

        private BsonDocument[] GetTrayByUserIdDocumentIdPipeline(string userId, string documentId)
        {
            var matchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("user", userId));
            var unWindAggregation = MongoDBAggregationExtension.UnWind(new("$inputTray"));
            var matchAggregation2 = MongoDBAggregationExtension.Match(new BsonDocument("inputTray.documentId", documentId));
            var projectAggregation = MongoDBAggregationExtension.Project(new()
                {
                    { "_id", 0 },
                    {"dossierId","$inputTray.dossierId" },
                    {"documentId","$inputTray.documentId" }
                });

            return new BsonDocument[] { matchAggregation, unWindAggregation, matchAggregation2, projectAggregation};
        }

        private PipelineDefinition<Tray, InputTrayResponse> GetInputTrayPipeline(string user)
        {
            var dossierInputLookupPipeline = DossierInputLookUpPipeline();

            var conditionalFilter = MongoDBAggregationExtension.Eq(new BsonArray() { "$$item.documentId", MongoDBAggregationExtension.ObjectId("$inputTray.documentId") });

            var filter = MongoDBAggregationExtension.Filter("$documentTray.documents", conditionalFilter, "item");

            var varDeclaration = MongoDBAggregationExtension.Let(new BsonDocument("document", filter),
                    MongoDBAggregationExtension.In("$arrayElemAt", new BsonArray { "$$document", 0 }));


            var projectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "Document", varDeclaration },
                { "InputTray", 1 },
                { "DossierType", "$documentTray.type" },
                { "Client", "$documentTray.client" }
            });

            var userFilter = Builders<Tray>.Filter.Eq("user", user).ToBsonDocument();
            var userFilterMatchAggregation = MongoDBAggregationExtension.Match(userFilter);

            var unWindAggregation1 = MongoDBAggregationExtension.UnWind(new("inputTray"));
            var unWindAggregation2 = MongoDBAggregationExtension.UnWind(new("documentTray"));

            return new BsonDocument[] { userFilterMatchAggregation, unWindAggregation1, dossierInputLookupPipeline, unWindAggregation2, projectAggregation };

        }

        private BsonDocument[] GetWorkloadByRolePipeline(string role)
        {
            var addFieldsAggregation = MongoDBAggregationExtension.AddFields(new()
            {
                { "totalTrays", MongoDBAggregationExtension.Add(new List<BsonValue>() { MongoDBAggregationExtension.Size("$inputTray"), MongoDBAggregationExtension.Size("$outputTray")  })   }
            });

            var sortAggregation = MongoDBAggregationExtension.Sort(new BsonDocument("totalTrays", 1 ));

            var lookupAggregation = GetWorkloadRolePipelineLookup();

            var unwindAggregation = MongoDBAggregationExtension.UnWind(new("$userInfo"));

            var matchAggregation = MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$userInfo.rol", role})));

            var unsetAggregation = MongoDBAggregationExtension.UnSet("_id");

            var projectAggregation = MongoDBAggregationExtension.Project(new()
                {
                    { "UserId", "$user" },
                    {"UserName","$userInfo.data.name" },
                    {"UserLastName","$userInfo.data.lastName" },
                    { "Quantity", "$totalTrays" }
                });

            return new BsonDocument[] { addFieldsAggregation, sortAggregation, lookupAggregation, unwindAggregation, matchAggregation, unsetAggregation, projectAggregation };
        }

        private BsonDocument[] GetTrayPipeline(string user)
        {
            var dossierLookupPipeline = DossierLookUpPipeline();
            var documentLookupPipeline = DocumentLookUpPipeline();

            ///* Group para agrupar los documentos en un array */

            var groupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$documentTray._id" },
                { "client", MongoDBAggregationExtension.First("$documentTray.client") },
                { "type", MongoDBAggregationExtension.First("$documentTray.type") },
                { "documentObjects", MongoDBAggregationExtension.Push("$documentObjects") },
                { "outputTray", MongoDBAggregationExtension.First("$outputTray") },
                { "inputTray", MongoDBAggregationExtension.First("$inputTray") },
                { "trayId", MongoDBAggregationExtension.First("$_id") }
            });

            ///* Proyección para devolver los datos que se necesitan */

            var conditionalFilter = MongoDBAggregationExtension.Eq(new BsonArray() { "$$item._id", MongoDBAggregationExtension.ObjectId("$outputTray.documentId") });

            var filter = MongoDBAggregationExtension.Filter("$documentObjects", conditionalFilter, "item");

            var declaredVariable = MongoDBAggregationExtension.Let(new BsonDocument("document", filter),
                new BsonDocument("$arrayElemAt", new BsonArray { "$$document", 0 }));


            var documentProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "document", declaredVariable },
                { "documentObjects", "$documentObjects" },
                { "type", "$type" },
                { "_id", "$_id" },
                { "client", "$client" },
                { "inputTray", 1 },
                { "trayId", 1 }
            });

            var outputDossierProjectAux = new BsonDocument
            {
                { "dossierId", "$_id" },
                { "client", "$client" },
                { "type", "$type" },
                { "documentObjects", "$documentObjects" },
                { "document", "$document" }
            };
            /* Proyección para crear el atributo de expediente salida*/
            var projectOutputCreationAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "trayId", 1 },
                { "inputTray", 1 },
                { "outputDossier", outputDossierProjectAux }
            });

            /* Group para unificar todos los expedientes de la bandeja de salida*/
            var groupOutputTrayDossierAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$trayId" },
                { "inputTray", MongoDBAggregationExtension.First("$inputTray") },
                { "outputDossier", MongoDBAggregationExtension.Push("$outputDossier") }
            });

            /* Lookup para los expedientes de la bandeja de entrada*/
            var dossierInputTrayLookUpAggregation = DossierInputTrayLookUpPipeline();

            /* Group para la bandeja de entrada y agrupos los elementos en un array*/
            var inputGroupAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$documentTray._id" },
                { "client", MongoDBAggregationExtension.First("$documentTray.client") },
                { "type", MongoDBAggregationExtension.First("$documentTray.type") },
                { "documentObjects", MongoDBAggregationExtension.Push("$documentObjects") },
                { "outputDossier", MongoDBAggregationExtension.First("$outputDossier") },
                { "inputTray", MongoDBAggregationExtension.First("$inputTray") },
                { "trayId", MongoDBAggregationExtension.First("$_id") }
            });

            /* Proyección para encontrar el documento de entrada */

            var conditionalInputFilter = MongoDBAggregationExtension.Eq(new BsonArray { "$$item._id", MongoDBAggregationExtension.ObjectId("$inputTray.documentId") });

            var inputfilter = MongoDBAggregationExtension.Filter("$documentObjects", conditionalInputFilter, "item");

            var inputVarDeclaration = MongoDBAggregationExtension.Let(new BsonDocument("document", inputfilter),
                new BsonDocument("$arrayElemAt", new BsonArray { "$$document", 0 }));

            var inputProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "document", inputVarDeclaration },
                { "documentObjects", "$documentObjects" },
                { "type", "$type" },
                { "_id", "$_id" },
                { "client", "$client" },
                { "inputTray", 1 },
                { "trayId", 1 },
                { "outputDossier", 1 }
            });

            var inputDossierProjectBsonAux = new BsonDocument()
            {
                { "dossierId", "$_id" },
                { "client", "$client" },
                { "type", "$type" },
                { "documentObjects", "$documentObjects" },
                { "document", "$document" }
            };

            /* Proyección para crear el expedienteentrada*/
            var inputDossierProjectAggregation = MongoDBAggregationExtension.Project(new()
            {
                { "_id", 0 },
                { "Id", "$trayId"},
                { "OutputDossier", "$outputDossier" },
                { "InputDossier", inputDossierProjectBsonAux }
            });

            /* Group final para la unión de los expedientes */
            var groupFinalAggregation = MongoDBAggregationExtension.Group(new()
            {
                { "_id", "$Id" },
                { "InputDossier", MongoDBAggregationExtension.Push("$InputDossier") },
                { "OutputDossier", MongoDBAggregationExtension.First("$OutputDossier") },
            });

            var userFilterMatchAggregation = MongoDBAggregationExtension.Match(new BsonDocument("user", user));


            var unWindAggregation0 = MongoDBAggregationExtension.UnWind(new("$outputTray", preserveNullAndEmptyArrays: true));
            var unWindAggregation1 = MongoDBAggregationExtension.UnWind(new("$documentTray", preserveNullAndEmptyArrays: true));
            var unWindAggregation2 = MongoDBAggregationExtension.UnWind(new("$documentTray.documents", preserveNullAndEmptyArrays: true));
            var unWindAggregation3 = MongoDBAggregationExtension.UnWind(new("$documentObjects", preserveNullAndEmptyArrays: true));
            var unWindAggregation4 = MongoDBAggregationExtension.UnWind(new("$inputTray", preserveNullAndEmptyArrays: true));

            return new BsonDocument[] { userFilterMatchAggregation, unWindAggregation0, dossierLookupPipeline, unWindAggregation1,
                unWindAggregation2,  documentLookupPipeline, unWindAggregation3, groupAggregation, documentProjectAggregation,
                projectOutputCreationAggregation,
                groupOutputTrayDossierAggregation, unWindAggregation4, dossierInputTrayLookUpAggregation,
                unWindAggregation1, unWindAggregation2, documentLookupPipeline, unWindAggregation3,
                inputGroupAggregation, inputProjectAggregation, inputDossierProjectAggregation, groupFinalAggregation };
        }
        private static BsonDocument DossierInputTrayLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "dossierId", "$inputTray.dossierId" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$dossierId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "documentTray"));
        }

        private static BsonDocument DossierLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "dossierId", "$outputTray.dossierId" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$dossierId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "documentTray"));
        }

        private static BsonDocument DocumentLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "documentId", "$documentTray.documents.documentId" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$documentId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("documentos", letPipeline, lookUpPipeline, "documentObjects"));
        }
        private static BsonDocument DossierInputLookUpPipeline()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "dossierId", "$inputTray.dossierId" }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$dossierId") })))
            };

            return MongoDBAggregationExtension.Lookup(new("expedientes", letPipeline, lookUpPipeline, "documentTray"));
        }

        private static BsonDocument GetWorkloadRolePipelineLookup()
        {
            var letPipeline = new Dictionary<string, BsonValue>()
            {
                { "userObjId", MongoDBAggregationExtension.ObjectId("$user") }
            };

            var lookUpPipeline = new BsonArray()
            {
                  MongoDBAggregationExtension.Match(MongoDBAggregationExtension.Expr(MongoDBAggregationExtension
                                                       .Eq(new BsonArray { "$_id", MongoDBAggregationExtension.ObjectId("$$userObjId") }))),
                MongoDBAggregationExtension.Project(new()
                {
                    { "_id", 1 },
                    {"data.name",1 },
                    {"data.lastName",1 },
                    { "rol", 1 }
                })
            };

            return MongoDBAggregationExtension.Lookup(new("usuarios", letPipeline, lookUpPipeline, "userInfo"));

        }
        #endregion
    }
}
