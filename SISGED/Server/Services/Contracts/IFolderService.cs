using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Contracts
{
    public interface IFolderService
    {
        Task<IEnumerable<Folder>> GetAsync();
    }
}
