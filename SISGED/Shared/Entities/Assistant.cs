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
        [BsonElement("idexpediente")]
        public string DossierId { get; set; } = default!;
        [BsonElement("pasos")]
        public List<AssistantStep> Steps { get; set; } = default!;
        [BsonElement("paso")]
        public int Step { get; set; } = 0;
        [BsonElement("subpaso")]
        public int Substep { get; set; } = 0;
        [BsonElement("tipodocumento")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("tipoexpediente")]
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
            var currentStep = GetCurrentAssistantStep()
                                   .Documents
                                   .Find(document => document.Type == DocumentType)!;

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
            Step = 0;
        }

        public bool IsLastSubStep() => Substep == GetCurrentDocumentStep().Substeps.Count - 1;
        public bool IsLastStep() => Step == GetCurrentStep().Steps.Count - 1;
        public bool IsLastDocument() => FindDocumentIndex() == GetCurrentAssistantStep().Documents.Count - 1;
    }

    public class AssistantStep
    {
        [BsonElement("nombreexpediente")]
        public string DossierName { get; set; } = default!;
        [BsonElement("documentos")]
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
    }
}
