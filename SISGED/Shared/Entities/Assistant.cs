using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Assistant
    {
        public Assistant()
        {

        }

        public Assistant(string dossierId, string dossierType, List<AssistantStep> steps)
        {
            DossierId = dossierId;
            DossierType = dossierType;
            Steps = steps;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("dossierId")]
        public string DossierId { get; set; } = default!;
        [BsonElement("steps")]
        public List<AssistantStep> Steps { get; set; } = default!;
        [BsonElement("step")]
        public int Step { get; set; } = 0;
        [BsonElement("substep")]
        public int Substep { get; set; } = 0;
        [BsonElement("documentType")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("dossierType")]
        public string DossierType { get; set; } = default!;

        public string GetFirstDocument() => Steps.First().Documents.First().Type;

        public string GetCurrentMessage()
        {
            string currentMessage = GetCurrentDocumentStep()
                                            .Substeps.ElementAt(Substep)
                                            .Description;

            return currentMessage;
        }

        public DocumentStep GetCurrentDocumentStep()
        {
            var currentDocumentStep = GetCurrentStep()!
                                            .Steps
                                            .ElementAt(Step);


            return currentDocumentStep;
        }

        public StepDocument GetCurrentStep()
        {
            var currentStep = GetCurrentAssistantStep().GetDocument(DocumentType);

            return currentStep;
        }

        public AssistantStep GetCurrentAssistantStep()
        {
            var currentAssistantStep = Steps.Find(step => step.DossierName == DossierType)!;

            return currentAssistantStep;
        }

        public int FindDocumentIndex()
        {
            int documentIndex = GetCurrentAssistantStep()
                                     .Documents
                                     .FindIndex(document => document.Type == DocumentType);

            return documentIndex;
        }

        public void UpdateNextStep()
        {
            int newStepIndex = GetCurrentStep().GetNextStepIndex(Step);

            Step = newStepIndex;
        }

        public void UpdateNextDocument(string documentType)
        {
            DocumentType = documentType;

            var stepDocument = GetCurrentStep();

            Step = stepDocument.Steps.First().Index;
        }

        public void UpdateNextDocumentStep()
        {
            var stepDocument = GetCurrentStep();

            Step = stepDocument.GetNextStepIndex(Step);
        }

        public bool IsLastSubStep() => Substep == GetCurrentDocumentStep().Substeps.Count;
        public bool IsLastStep() => Step == GetCurrentStep().Steps.Count - 1;
        public bool IsLastDocument() => FindDocumentIndex() == GetCurrentAssistantStep().Documents.Count - 1;
        public DocumentStep GetDocumentStep(string documentType) => GetCurrentAssistantStep()
                                                                        .GetDocument(documentType)
                                                                        .Steps
                                                                        .Last();
    }

    public class AssistantStep
    {
        [BsonElement("dossierName")]
        public string DossierName { get; set; } = default!;
        [BsonElement("documents")]
        public List<StepDocument> Documents { get; set; } = default!;

        public AssistantStep()
        {

        }

        public AssistantStep(string dossierName)
        {
            DossierName = dossierName;
        }

        public AssistantStep(string dossierName, List<StepDocument> documents) : this(dossierName)
        {
            Documents = documents;
        }

        public StepDocument GetDocument(string documentType) => Documents.FirstOrDefault(document => document.Type == documentType)!;
    }
}
