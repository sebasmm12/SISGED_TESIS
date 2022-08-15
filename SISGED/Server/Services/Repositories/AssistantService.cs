using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Services.Repositories
{
    public class AssistantService : IAssistantService
    {
        private readonly IMongoCollection<Assistant> _assistantCollection;
        private readonly IStepService _stepService;
        public string CollectionName => "asistente";
        public AssistantService(IMongoDatabase mongoDatabase)
        {
            _assistantCollection = mongoDatabase.GetCollection<Assistant>(CollectionName);
        }
        public async Task<Assistant> CreateAsync(Assistant assistant)
        {
            Step steps = await _stepService.GetStepByDossierNameAsync(assistant.Steps.DossierName);

            assistant.Steps = new AssistantStep { Documents = steps.Documents, DossierName = steps.DossierName};
            assistant.Step = 0;
            assistant.Substep = 0;
            assistant.DocumentType = "SolicitudInicial";

            await _assistantCollection.InsertOneAsync(assistant);

            return assistant;
        }

        public async Task<Assistant> GetAssistantAsync(string dossierId)
        {
            Assistant assistant = await _assistantCollection.Find(x => x.DossierId == dossierId).FirstAsync();
            return assistant;
        }

        public async Task<Assistant> UpdateAsync(StepsUpdateRequest stepUpdateRequest)
        {
            FilterDefinition<Assistant> queryUpdate = Builders<Assistant>.Filter.Eq("idexpediente", stepUpdateRequest.DossierId);

            UpdateDefinition<Assistant> updateAsistente = Builders<Assistant>.Update
                                                                                .Set("paso", stepUpdateRequest.Step)
                                                                                .Set("subpaso", stepUpdateRequest.Substep)
                                                                                .Set("tipodocumento", stepUpdateRequest.DocumentType)
                                                                                .Set("pasos.documentos.$[doc].pasos.$[pas].fechainicio", stepUpdateRequest.StartDate)
                                                                                .Set("pasos.documentos.$[doc].pasos.$[pas].fechalimite", stepUpdateRequest.EndDate);


            var arrayFilter = new List<ArrayFilterDefinition>();

            arrayFilter.Add(new BsonDocumentArrayFilterDefinition<Assistant>(new BsonDocument("doc.tipo", stepUpdateRequest.DocumentType)));
            arrayFilter.Add(new BsonDocumentArrayFilterDefinition<Assistant>(new BsonDocument("pas.indice", stepUpdateRequest.Step)));

            Assistant asistenteActualizado = await _assistantCollection.FindOneAndUpdateAsync(queryUpdate, updateAsistente, new FindOneAndUpdateOptions<Assistant>
            {
                ReturnDocument = ReturnDocument.After,
                ArrayFilters = arrayFilter
            });

            return asistenteActualizado;
        }

        public async Task<Assistant> UpdateFinallyAsync(StepsUpdateRequest stepUpdateRequest)
        {
            FilterDefinition<Assistant> queryUpdate = Builders<Assistant>.Filter.Eq("idexpediente", stepUpdateRequest.DossierId);

            UpdateDefinition<Assistant> updateAsistente = Builders<Assistant>.Update
                                                                                .Set("paso", stepUpdateRequest.Step)
                                                                                .Set("subpaso", stepUpdateRequest.Substep)
                                                                                .Set("tipodocumento", stepUpdateRequest.DocumentType)
                                                                                .Set("pasos.documentos.$[doc].pasos.$[pas].fechafin", stepUpdateRequest.EndDate);

            var arrayFilter = new List<ArrayFilterDefinition>();

            arrayFilter.Add(new BsonDocumentArrayFilterDefinition<Assistant>(new BsonDocument("doc.tipo", stepUpdateRequest.OldDocumentType)));
            arrayFilter.Add(new BsonDocumentArrayFilterDefinition<Assistant>(new BsonDocument("pas.indice", stepUpdateRequest.OldStep)));

            Assistant asistenteActualizado = await _assistantCollection.FindOneAndUpdateAsync(queryUpdate, updateAsistente, new FindOneAndUpdateOptions<Assistant>
            {
                ReturnDocument = ReturnDocument.After,
                ArrayFilters = arrayFilter
            });

            return asistenteActualizado;
        }

        public async Task<Assistant> UpdateInitialRequestAsync(Assistant assistant, string dossierName)
        {
            Step steps = await _stepService.GetStepByDossierNameAsync(dossierName);

            steps.Documents.Find(x => x.Type == assistant.DocumentType)!
                .Steps.Find(x => x.Index == assistant.Step - 1)!.StartDate = assistant.Steps.Documents.ElementAt(0).Steps.ElementAt(0).StartDate;

            steps.Documents.Find(x => x.Type == assistant.DocumentType)!
                .Steps.Find(x => x.Index == assistant.Step - 1)!.EndDate = assistant.Steps.Documents.ElementAt(0).Steps.ElementAt(0).EndDate;

            steps.Documents.Find(x => x.Type == assistant.DocumentType)!
            .Steps.Find(x => x.Index == assistant.Step - 1)!.DueDate = assistant.Steps.Documents.ElementAt(0).Steps.ElementAt(0).DueDate;

            FilterDefinition<Assistant> queryUpdate = Builders<Assistant>.Filter.Eq("idexpediente", assistant.DossierId);

            UpdateDefinition<Assistant> updateAssistant = Builders<Assistant>.Update
                                                                                .Set("paso", assistant.Step)
                                                                                .Set("subpaso", assistant.Substep)
                                                                                .Set("tipodocumento", assistant.DocumentType)
                                                                                .Set("pasos", new AssistantStep { Documents = steps.Documents, DossierName = steps.DossierName });


            Assistant updatedAssistant = await _assistantCollection.FindOneAndUpdateAsync(queryUpdate, updateAssistant, new FindOneAndUpdateOptions<Assistant>
            {
                ReturnDocument = ReturnDocument.After
            });

            return updatedAssistant;
        }

        public async Task<Assistant> UpdateNormalAsync(StepsUpdateRequest stepUpdateRequest)
        {
            FilterDefinition<Assistant> queryUpdate = Builders<Assistant>.Filter.Eq("idexpediente", stepUpdateRequest.DossierId);

            UpdateDefinition<Assistant> updateAssistant = Builders<Assistant>.Update
                                                                                .Set("paso", stepUpdateRequest.Step)
                                                                                .Set("subpaso", stepUpdateRequest.Substep)
                                                                                .Set("tipodocumento", stepUpdateRequest.DocumentType);

            Assistant updatedAssistant = await _assistantCollection.FindOneAndUpdateAsync(queryUpdate, updateAssistant, new FindOneAndUpdateOptions<Assistant>
            {
                ReturnDocument = ReturnDocument.After,
            });

            return updatedAssistant;
        }
    }
}
