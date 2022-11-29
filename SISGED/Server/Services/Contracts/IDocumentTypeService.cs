using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentTypeService : IGenericService
    {
        Task<IEnumerable<DocumentType>> GetDocumentTypesAsync(string type);
    }
}
