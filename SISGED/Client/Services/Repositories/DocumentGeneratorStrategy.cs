using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentGeneratorStrategy
    {
        private readonly IEnumerable<IDocumentRender> documents = new List<IDocumentRender>()
        {
            new ComplaintRequestGeneration(),
            new DictumGeneration(),
            new SolicitorDossierShipmentGeneration(),
            new SolicitorDossierRequestGeneration(),
            new DisciplinaryOpennessGeneration(),
            new ResolutionGeneration()
        };

        public IDocumentRender GetDocument(string documentType)
        {
            return documents.FirstOrDefault(document => document.DocumentType == documentType)!;
        }
    }
}
