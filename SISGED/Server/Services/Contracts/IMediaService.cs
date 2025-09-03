using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Contracts
{
    public interface IMediaService
    {
        Task<IEnumerable<string>> SaveFilesAsync(IEnumerable<MediaRegisterDTO> files, string containerName);
        Task<string> SaveFileAsync(MediaRegisterDTO file, string containerName);
        Task<Tuple<string, string>[]> GetFilesAsync(IEnumerable<string> annexUrls, string containerName);
    }
}
