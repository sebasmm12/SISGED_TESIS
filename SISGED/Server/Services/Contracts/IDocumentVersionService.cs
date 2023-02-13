using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentVersionService : IGenericService
    {
        Task<IEnumerable<ContentVersion>> GetContentVersionsByDocumentIdAsync(string documentId);
    }
}
