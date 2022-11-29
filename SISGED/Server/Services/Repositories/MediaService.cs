using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Repositories
{
    public class MediaService : IMediaService
    {
        private readonly IFileStorageService _fileStorageApplication;

        public MediaService(IFileStorageService fileStorageApplication)
        {
            _fileStorageApplication = fileStorageApplication;
        }

        public async Task<string> SaveFileAsync(MediaRegisterDTO file, string containerName)
        {
            if (string.IsNullOrWhiteSpace(file.Content)) throw new Exception($"El archivo a registrar debe tener contenido");

            var fileBytes = Convert.FromBase64String(file.Content);

            var fileRegister = new FileRegisterDTO(fileBytes, file.Extension, containerName);

            string fileUrl = await _fileStorageApplication.SaveFileAsync(fileRegister);

            return fileUrl;
        }

        public async Task<IEnumerable<string>> SaveFilesAsync(IEnumerable<MediaRegisterDTO> files, string containerName)
        {
            
            var urlTasks = files.Select(file => SaveFileAsync(file, containerName));

            var urls = await Task.WhenAll(urlTasks);

            return urls;
        }
    }
}
