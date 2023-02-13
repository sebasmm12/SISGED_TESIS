using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentProcess;

namespace SISGED.Server.Services.Contracts
{
    public interface IDocumentProcessService : IGenericService
    {
        Task<IEnumerable<DocumentProcessInfo>> GetProcessesByDocumentIdAsync(string documentId);
    }
}
