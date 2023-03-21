using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Step
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("nombreexpediente")]
        public string DossierName { get; set; } = default!;
        [BsonElement("documentos")]
        public List<StepDocument> Documents { get; set; } = default!;

        public StepDocument GetFirstDocument()
        {
            return Documents.FirstOrDefault()!;
        }
    }

    public class StepDocument
    {
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("pasos")]
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
        [BsonElement("indice")]
        public int Index { get; set; }
        [BsonElement("idaccion")]
        public string ActionId { get; set; } = default!;
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("fechainicio")]
        public DateTime? StartDate { get; set; }
        [BsonElement("fechafin")]
        public DateTime? EndDate { get; set; }
        [BsonElement("fechalimite")]
        public DateTime? DueDate { get; set; }
        [BsonElement("dias")]
        public int Days { get; set; }
        [BsonElement("documentosregistrados")]
        public List<string> RegisteredDocuments { get; set; } = new();
        [BsonElement("rolreceptor")]
        public string? ReceiverRole { get; set; }
        [BsonElement("esopcional")]
        public bool IsOptional { get; set; }
        [BsonElement("subpasos")]
        public List<Substep> Substeps { get; set; } = default!;

    }

    public class Substep
    {
        [BsonElement("indice")]
        public int Index { get; set; }
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
    }
}
