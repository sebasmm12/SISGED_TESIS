using MongoDB.Bson;
using MongoDB.Driver;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Services.Repositories
{
    public class AssistantService : IAssistantService
    {
        private readonly IMongoCollection<Assistant> _assistantCollection;

        private readonly IStepService _stepService;

        public string CollectionName => "asistente";

        public AssistantService(IMongoDatabase mongoDatabase, IStepService stepService)
        {
            _assistantCollection = mongoDatabase.GetCollection<Assistant>(CollectionName);
            _stepService = stepService;
        }


        public async Task<Assistant> GetAssistantAsync(string assistantId)
        {
            var assistant = await _assistantCollection.Find(assistant => assistant.Id == assistantId).FirstOrDefaultAsync();

            if (assistant is null) throw new Exception($"No se pudo encontrar el asistante con identificador {assistantId}");

            return assistant;
        }

        public async Task<Assistant> CreateAsync(Assistant assistant)
        {
            await _assistantCollection.InsertOneAsync(assistant);

            if (assistant.Id is null) throw new Exception($"No se pudo registrar el asistente para el expediente con identificador {assistant.DossierType}");

            return assistant;
        }

        public async Task<Assistant> GetAssistantByDossierAsync(string dossierId)
        {
            Assistant assistant = await _assistantCollection.Find(x => x.DossierId == dossierId).FirstAsync();
            return assistant;
        }

        public async Task<Assistant> UpdateAssistantStepStartDateAsync(AssistantStepStartDateUpdateDTO stepStartDateUpdateRequest)
        {
            var assistantQuery = Builders<Assistant>.Filter.Eq(assistant => assistant.Id, stepStartDateUpdateRequest.Id);

            var assistantUpdate = Builders<Assistant>.Update
                                                        .Set("pasos.$[dossier].documentos.$[document].pasos.$[step].fechainicio", stepStartDateUpdateRequest.StartDate)
                                                        .Set("pasos.$[dossier].documentos.$[document].pasos.$[step].fechalimite", stepStartDateUpdateRequest.LimitDate);

            var assistantArrayFilters = GetAssistantStepUpdateFilters(stepStartDateUpdateRequest.Assistant);

            var updatedAssistant = await _assistantCollection.FindOneAndUpdateAsync(assistantQuery, assistantUpdate,
                                                                                    new FindOneAndUpdateOptions<Assistant>
                                                                                    {
                                                                                        ReturnDocument = ReturnDocument.After,
                                                                                        ArrayFilters = assistantArrayFilters
                                                                                    });

            if (updatedAssistant is null) throw new Exception($"No se pudo actualizar las fechas inicio y límite del paso actual del asistente con identificador {stepStartDateUpdateRequest.Id}");

            return updatedAssistant;
        }


        public async Task<Assistant> UpdateAssistantStepAsync(AssistantStepUpdateDTO assistantStepUpdate)
        {
            var assistantQuery = Builders<Assistant>.Filter.Eq(assistant => assistant.Id, assistantStepUpdate.Id);


            var assistantUpdate = Builders<Assistant>.Update
                                                        .Set("pasos.$[dossier].documentos.$[document].pasos.$[step].fechafin", assistantStepUpdate.EndDate)
                                                        .Set("paso", assistantStepUpdate.NewAssistantStep.Step)
                                                        .Set("tipodocumento", assistantStepUpdate.NewAssistantStep.DocumentType);

            var assistantArrayFilters = GetAssistantStepUpdateFilters(assistantStepUpdate.LastAssistantStep);

            var updatedAssistant = await _assistantCollection.FindOneAndUpdateAsync(assistantQuery, assistantUpdate,
                                                                                   new FindOneAndUpdateOptions<Assistant>
                                                                                   {
                                                                                       ReturnDocument = ReturnDocument.After,
                                                                                       ArrayFilters = assistantArrayFilters
                                                                                   });

            if (updatedAssistant is null) throw new Exception($"No se pudo actualizar el paso del asistente con identificador {assistantStepUpdate.Id}");

            return updatedAssistant;

        }

        public async Task<Assistant> UpdateAssistantDossierAsync(Assistant assistant, AssistantDossierUpdateDTO assistantDossierUpdate)
        {
            var assistantQuery = Builders<Assistant>.Filter.Eq(assistant => assistant.Id, assistantDossierUpdate.Id);

            var assistantUpdate = GetAssistantDossierUpdateBuilder<Assistant>(assistantDossierUpdate);

            var updatedAssistant = await _assistantCollection.FindOneAndUpdateAsync(assistantQuery, assistantUpdate,
                                                                                  new FindOneAndUpdateOptions<Assistant>
                                                                                  {
                                                                                      ReturnDocument = ReturnDocument.After,
                                                                                  });

            if (updatedAssistant is null) throw new Exception($"No se pudo añadir el expediente de tipo {assistantDossierUpdate.DossierType} al asistente con identificador {assistant.Id}");

            return updatedAssistant;
        }

        public async Task<Assistant> UpdateAssistantDocumentLastStepAsync(string assistantId, AssistantStepDTO assistantStepDTO)
        {
            var assistantQuery = Builders<Assistant>.Filter.Eq(assistant => assistant.Id, assistantId);

            var assistantUpdateBuilder = Builders<Assistant>.Update
                                                        .Set("pasos.$[dossier].documentos.$[document].pasos.$[step].fechainicio", BsonNull.Value)
                                                        .Set("pasos.$[dossier].documentos.$[document].pasos.$[step].fechalimite", BsonNull.Value);

            var assistantArrayFilters = GetAssistantStepUpdateFilters(assistantStepDTO);

            var updatedAssistant = await _assistantCollection.FindOneAndUpdateAsync(assistantQuery, assistantUpdateBuilder,
                                                                                  new FindOneAndUpdateOptions<Assistant>
                                                                                  {
                                                                                      ReturnDocument = ReturnDocument.After,
                                                                                      ArrayFilters = assistantArrayFilters
                                                                                  });

            if (updatedAssistant is null) throw new Exception($"No se pudo actualizar la información del paso del asistente con identificador { assistantId }");

            return updatedAssistant;

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
            Step steps = await _stepService.GetStepByDossierTypeAsync(dossierName);

            steps.Documents.Find(x => x.Type == assistant.DocumentType)!
                .Steps.Find(x => x.Index == assistant.Step - 1)!.StartDate = assistant.Steps.First().Documents.ElementAt(0).Steps.ElementAt(0).StartDate;

            steps.Documents.Find(x => x.Type == assistant.DocumentType)!
                .Steps.Find(x => x.Index == assistant.Step - 1)!.EndDate = assistant.Steps.First().Documents.ElementAt(0).Steps.ElementAt(0).EndDate;

            steps.Documents.Find(x => x.Type == assistant.DocumentType)!
            .Steps.Find(x => x.Index == assistant.Step - 1)!.DueDate = assistant.Steps.First().Documents.ElementAt(0).Steps.ElementAt(0).DueDate;

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

        #region private methods
        private static UpdateDefinition<T> GetAssistantDossierUpdateBuilder<T>(AssistantDossierUpdateDTO assistantDossierUpdate) where T : Assistant
        {

            var assistantUpdateBuilder = Builders<T>.Update
                                                        .Set("paso", assistantDossierUpdate.Step)
                                                        .Set("tipodocumento", assistantDossierUpdate.DocumentType)
                                                        .Set("tipoexpediente", assistantDossierUpdate.DossierType)
                                                        .Push("pasos", assistantDossierUpdate.Steps);

            return assistantUpdateBuilder;
        }

        private List<ArrayFilterDefinition> GetAssistantStepUpdateFilters(AssistantStepDTO assistantStepDTO)
        {
            var assistantArrayFilters = new List<ArrayFilterDefinition>()
            {
                MongoDBAggregationExtension.GetArrayFilterDefinition<Assistant>("dossier.nombreexpediente", assistantStepDTO.DossierType),
                MongoDBAggregationExtension.GetArrayFilterDefinition<Assistant>("document.tipo", assistantStepDTO.DocumentType),
                MongoDBAggregationExtension.GetArrayFilterDefinition<Assistant>("step.indice", assistantStepDTO.Step)
            };

            return assistantArrayFilters;
        }
        #endregion
    }
}
