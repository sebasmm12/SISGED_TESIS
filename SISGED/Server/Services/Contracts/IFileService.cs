using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Contracts
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(FileStreamRegisterDTO fileRegisterDTO);
        Task<string?> UpdateImageAsync(FileStreamUpdateDTO fileUpdateDTO);
    }
}
