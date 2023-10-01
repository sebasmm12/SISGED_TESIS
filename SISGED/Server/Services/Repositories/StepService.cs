using AutoMapper;
using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;
using SISGED.Shared.Models.Responses.Step;
using StepGenericModel = SISGED.Shared.Models.Generics.Step;

namespace SISGED.Server.Services.Repositories
{
    public class StepService : IStepService
    {
        private readonly IMongoCollection<Step> _stepsCollection;
        private readonly IMapper _mapper;
        public string CollectionName => "pasos";
        public StepService(IMongoDatabase mongoDatabase, IMapper mapper)
        {
            _stepsCollection = mongoDatabase.GetCollection<Step>(CollectionName);
            _mapper = mapper;
        }

        public async Task<Step> GetStepByDossierTypeAsync(string dossierName)
        {
            var steps = await _stepsCollection.Find(step => step.DossierName == dossierName).FirstOrDefaultAsync();

            if (steps is null) throw new Exception($"No se pudo encontrar los pasos registrados del expediente {dossierName}");

            return steps;
        }

        public async Task<Step> GetStepByIdAsync(string stepId)
        {
            var steps = await _stepsCollection.Find(step => step.Id == stepId).FirstOrDefaultAsync();

            if (steps is null) throw new Exception($"No se pudo encontrar los pasos registrados del expediente con identificador {stepId}");

            return steps;
        }

        public async Task<IEnumerable<DossierStepsResponse>> GetStepRequestAsync()
        {
            var steps = await GetStepsAsync();

            if (!steps.Any()) return new List<DossierStepsResponse>();

            var stepsRequest = steps.Select(step =>
            {
                var stepRequest = _mapper.Map<DossierStepsResponse>(step);

                stepRequest.Documents = step.Documents.Select(GetDocumentRequest).ToList();

                return stepRequest;

            }).ToList();

            return stepsRequest;
        }

        private StepGenericModel.StepDocument GetDocumentRequest(StepDocument stepDocument, int stepDocumentIndex)
        {
            var stepDocumentRequest = new StepGenericModel.StepDocument(stepDocumentIndex);

            stepDocumentRequest = _mapper.Map(stepDocument, stepDocumentRequest);

            return stepDocumentRequest;
        }

        public async Task<List<Step>> GetStepsAsync()
        {
            var steps = await _stepsCollection.Find(x => true).ToListAsync();

            if (steps is null) throw new Exception("No se pudo obtener la hoja de ruta de los expedientes registrados");

            return steps;
        }

        public async Task UpdateStepAsync(StepUpdateRequest stepUpdateRequest)
        {
            var step = _mapper.Map<Step>(stepUpdateRequest);

            var filter = Builders<Step>.Filter.Eq("id", step.Id);
            var update = SetStepInformation(step);

            var updatedUser = await _stepsCollection.UpdateOneAsync(filter, update);

            if (updatedUser is null) throw new Exception($"No se pudo actualizar la hoja de ruta del expediente {stepUpdateRequest.DossierName}");
        }

        public async Task RegisterStepAsync(StepRegisterRequest stepRegisterRequest)
        {
            var step = _mapper.Map<Step>(stepRegisterRequest);

            await _stepsCollection.InsertOneAsync(step);

            if (step.Id is null) throw new Exception($"No se pudo registrar la hoja de ruta del expediente {stepRegisterRequest.DossierName}");
        }

        private static UpdateDefinition<Step> SetStepInformation(Step step)
        {
            return Builders<Step>.Update
                .Set("dossierName", step.DossierName)
                .Set("documents", step.Documents);
        }
    }
}
