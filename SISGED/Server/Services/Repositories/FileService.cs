using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Repositories
{
    public class FileService : IFileService
    {
        private readonly IFileStorageService _fileStorageService;

        public FileService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<string?> SaveFileAsync(FileStreamRegisterDTO fileStreamRegisterDTO)
        {
            if (string.IsNullOrEmpty(fileStreamRegisterDTO.Image)) return null;

            var file = Convert.FromBase64String(fileStreamRegisterDTO.Image);

            var fileRegisterDTO = new FileRegisterDTO(file, fileStreamRegisterDTO.Extension, fileStreamRegisterDTO.ContainerName);

            string fileUrl = await _fileStorageService.SaveFileAsync(fileRegisterDTO);

            return fileUrl;
        }

        public async Task<string?> UpdateImageAsync(FileStreamUpdateDTO fileStreamUpdateDTO)
        {
            if (string.IsNullOrEmpty(fileStreamUpdateDTO.NewImage) ||
               fileStreamUpdateDTO.CurrentImage == fileStreamUpdateDTO.NewImage) return fileStreamUpdateDTO.CurrentImage;

            var file = Convert.FromBase64String(fileStreamUpdateDTO.NewImage);

            var fileUpdateDTO = new FileUpdateDTO(file, fileStreamUpdateDTO.Extension, fileStreamUpdateDTO.ContainerName, fileStreamUpdateDTO.CurrentImage);

            string fileUrl = await _fileStorageService.UpdateFileAsync(fileUpdateDTO);

            return fileUrl;
        }
    }
}
