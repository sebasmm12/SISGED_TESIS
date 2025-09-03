using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories;

public class DocumentStrategy
{
    private readonly IEnumerable<IDocumentRender> documents = new List<IDocumentRender>
    {
        new BPNRequestDocument(),
        new SignExpeditionRequestDocument(),
        new ComplaintRequestDocument(),
        new DisciplinaryOpennessRequestDocument(),
        new SolicitorDossierRequestDocument(),
        new DictumDocument(),
        new SolicitorDossierShipmentDocument(),
        new DictumDocument(),
        new ResolutionDocument(),
        new SessionResolutionDocument()
    };

    private readonly IEnumerable<IDocumentRender> documentsToUpdate = new List<IDocumentRender>
    {
        new ComplaintRequestUpdateDocument()
    };

    public IDocumentRender GetDocument(string documentType)
    {
        return documents.FirstOrDefault(document => document.DocumentType == documentType)!;
    }

    public IDocumentRender GetDocumentToUpdate(string documentType)
    {
        return documentsToUpdate.FirstOrDefault(document => document.DocumentType == documentType)!;
    }
}