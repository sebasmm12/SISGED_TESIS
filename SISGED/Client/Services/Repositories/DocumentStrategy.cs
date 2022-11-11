using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentStrategy
    {
        private readonly IEnumerable<IDocumentRender> documents = new List<IDocumentRender>()
        {
            new BPNRequestDocument(),
            new SignExpeditionRequestDocument(),
            new ComplaintRequestDocument()
        };

        public IDocumentRender GetDocument(string documentType)
        {
            return documents.FirstOrDefault(document => document.DocumentType == documentType)!;
        }
    }
}
