using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Contracts
{
    public interface IFileStorageService
    {
        public Task<string> SaveFileAsync(FileRegisterDTO fileRegisterDTO);
        public Task DeleteFileAsync(FileEliminationDTO fileEliminationDTO);
        public Task<string> UpdateFileAsync(FileUpdateDTO fileUpdateDTO);
    }
}
