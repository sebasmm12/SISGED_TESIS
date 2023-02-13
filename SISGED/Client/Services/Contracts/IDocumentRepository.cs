using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IDocumentRepository
    {
        Type GetDocumentInfoType(string documentType);
        public IEnumerable<DocumentOption> GetDocumentTypesWithDossier();
    }
}
