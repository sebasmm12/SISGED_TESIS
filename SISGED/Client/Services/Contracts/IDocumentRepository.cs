using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IDocumentRepository
    {
        public IEnumerable<DocumentOption> GetDocumentTypesWithDossier();
    }
}
