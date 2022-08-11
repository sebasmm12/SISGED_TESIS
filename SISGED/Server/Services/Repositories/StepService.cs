using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Step;

namespace SISGED.Server.Services.Repositories
{
    public class StepService : IStepService
    {
        private readonly IMongoCollection<Steps> _stepsCollection;
        public string CollectionName => "pasos";
        public StepService(IMongoDatabase mongoDatabase)
        {
            _stepsCollection = mongoDatabase.GetCollection<Steps>(CollectionName);
        }

        public async Task<Steps> GetStepByDossierNameAsync(string dossierName)
        {
            var steps = await _stepsCollection.Find(step => step.DossierName == dossierName).FirstOrDefaultAsync();

            if (steps is null) throw new Exception("No se ha podido encontrar los pasos registrados del expediente");

            return steps;
        }

        public async Task<Steps> GetStepByIdAsync(string stepId)
        {
            var steps = await _stepsCollection.Find(step => step.Id == stepId).FirstOrDefaultAsync();

            if (steps is null) throw new Exception("No se ha podido encontrar los pasos registrados del expediente");

            return steps;
        }

        public async Task<List<StepsRequest>> GetStepRequestAsync()
        {
            List<Steps> steps = await _stepsCollection.Find(x => true).ToListAsync();
            List<StepsRequest> stepsrequest = new List<StepsRequest>();
            stepsrequest = (List<StepsRequest>)steps.Select(x => new StepsRequest()
            {
                Id = x.Id,
                DossierName = x.DossierName,
                Documents = (List<DocumentStepRequest>)x.Documents.Select((a, b) => new DocumentStepRequest()
                {
                    Uid = GenerateUID(),
                    Index = b,
                    Type = a.Type,
                    Steps = (List<StepRequest>)a.Steps.Select((c, d) => new StepRequest()
                    {
                        Uid = GenerateUID(),
                        Index = c.Index,
                        Name = c.Name,
                        Description = c.Description,
                        Days = c.Days,
                        Substep = (List<Substep>)c.Substep.Select((e, f) => new Substep()
                        {
                            Index = e.Index,
                            Description = e.Description
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).ToList();
            return stepsrequest;
        }

        public async Task<List<Steps>> GetStepsAsync()
        {
            List<Steps> steps = await _stepsCollection.Find(x => true).ToListAsync();
            return steps;
        }

        public async Task<StepsRequest> ModifyStep(StepsRequest stepsRequest)
        {
            Steps steps = new Steps()
            {
                Id = stepsRequest.Id,
                DossierName = stepsRequest.DossierName,
                Documents = stepsRequest.Documents.Select(x => new DocumentStep()
                {
                    Type = x.Type,
                    Steps = (List<Step>)x.Steps.Select((a, b) => new Step()
                    {
                        Index = b,
                        Name = a.Name,
                        Description = a.Description,
                        Days = a.Days,
                        Substep = (List<Substep>)a.Substep.Select((c, d) => new Substep()
                        {
                            Index = d,
                            Description = c.Description
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            var filter = Builders<Steps>.Filter.Eq("id", steps.Id);
            var update = Builders<Steps>.Update
                .Set("nombreexpediente", steps.DossierName)
                .Set("documentos", steps.Documents);
            await _stepsCollection.FindOneAndUpdateAsync<Pasos>(filter, update);
            return stepsRequest;
        }

        public async Task<StepsRequest> RegisterStepAsync(StepsRequest stepsRequest)
        {
            Steps steps = new Steps()
            {
                DossierName = stepsRequest.DossierName,
                Documents = stepsRequest.Documents.Select(x => new DocumentStep()
                {
                    Type = x.Type,
                    Steps = (List<Step>)x.Steps.Select((a, b) => new Step()
                    {
                        Index = b,
                        Name = a.Name,
                        Description = a.Description,
                        Days = a.Days,
                        Substep = (List<Substep>)a.Substep.Select((c, d) => new Substep()
                        {
                            Index = d,
                            Description = c.Description
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
            await _stepsCollection.InsertOneAsync(steps);
            stepsRequest.Id = steps.Id;
            return stepsRequest;
        }

        public string GenerateUID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
