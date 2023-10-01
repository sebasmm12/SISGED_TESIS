using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Step
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("dossierName")]
        public string DossierName { get; set; } = default!;
        [BsonElement("documents")]
        public List<StepDocument> Documents { get; set; } = default!;

        public StepDocument GetFirstDocument()
        {
            return Documents.FirstOrDefault()!;
        }
    }

    public class StepDocument
    {
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("steps")]
        public List<DocumentStep> Steps { get; set; } = default!;

        public int GetNextStepIndex(int currentStep)
        {
            var primarySteps =  Steps
                                .Where(step => !step.IsOptional)
                                .ToList();

            int currentStepIndex = primarySteps.FindIndex(step => step.Index == currentStep);

            var newStep = primarySteps.ElementAt(currentStepIndex + 1);

            return newStep.Index;
        }
    }
    public class DocumentStep
    {
        [BsonElement("index")]
        public int Index { get; set; }
        [BsonElement("actionId")]
        public string ActionId { get; set; } = default!;
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("startDate")]
        public DateTime? StartDate { get; set; }
        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }
        [BsonElement("dueDate")]
        public DateTime? DueDate { get; set; }
        [BsonElement("days")]
        public int Days { get; set; }
        [BsonElement("registeredDocuments")]
        public List<string> RegisteredDocuments { get; set; } = new();
        [BsonElement("receiverRole")]
        public string? ReceiverRole { get; set; }
        [BsonElement("isOptional")]
        public bool IsOptional { get; set; }
        [BsonElement("substeps")]
        public List<Substep> Substeps { get; set; } = default!;

    }

    public class Substep
    {
        [BsonElement("index")]
        public int Index { get; set; }
        [BsonElement("description")]
        public string Description { get; set; } = default!;
    }
}
