using Azure.Storage.Blobs;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Server.Services.Repositories
{
    public class AzureFileStorageService : IFileStorageService
    {
        private readonly string _connectionString;

        public AzureFileStorageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("FileStorageConnection:AzureStorage").Value;
        }

        public async Task DeleteFileAsync(FileEliminationDTO fileEliminationDTO)
        {
            if (string.IsNullOrEmpty(fileEliminationDTO.Url)) return;

            var client = await VerifyFileAsync(fileEliminationDTO.ContainerName);

            var fileName = Path.GetFileName(fileEliminationDTO.Url);

            var blob = client.GetBlobClient(fileName);
            
            await blob.DeleteIfExistsAsync();

        }

        public async Task<string> SaveFileAsync(FileRegisterDTO fileRegisterDTO)
        {
            var client = await VerifyFileAsync(fileRegisterDTO.ContainerName);

            client.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

            var fileName = $"{Guid.NewGuid()}";

            fileName += string.IsNullOrEmpty(fileRegisterDTO.Name) ? 
                $"{fileRegisterDTO.Extension}" : 
                $"_{fileRegisterDTO.Name}{fileRegisterDTO.Extension}";

            var blob = client.GetBlobClient(fileName);

            using var memoryStream = new MemoryStream(fileRegisterDTO.Content);

            await blob.UploadAsync(memoryStream);

            return blob.Uri.ToString();

        }

        public async Task<Tuple<string, string>> GetFileAsync(string url, string containerName)
        {
            using var memoryStream = new MemoryStream();

            var client = await VerifyFileAsync(containerName);

            var encryptedFileName = Path.GetFileName(url);

            var fileName = encryptedFileName.Contains("_") ? 
                encryptedFileName[(encryptedFileName.IndexOf("_") + 1)..] : 
                encryptedFileName;

            var blob = client.GetBlobClient(encryptedFileName);

            await blob.DownloadToAsync(memoryStream);

            memoryStream.Position = 0;

            var content = Convert.ToBase64String(memoryStream.ToArray());

            return new(content, fileName);
        }

        public async Task<string> UpdateFileAsync(FileUpdateDTO fileUpdateDTO)
        {
            await DeleteFileAsync(new FileEliminationDTO(fileUpdateDTO.Url, fileUpdateDTO.ContainerName));

            var fileUrl = await SaveFileAsync(new FileRegisterDTO(fileUpdateDTO.Content, fileUpdateDTO.Extension, fileUpdateDTO.ContainerName));

            if (string.IsNullOrEmpty(fileUrl)) throw new Exception("No se ha podido actualizar el archivo");

            return fileUrl;
        }

        private async Task<BlobContainerClient> VerifyFileAsync(string containerName)
        {
            var client = new BlobContainerClient(_connectionString, containerName);

            await client.CreateIfNotExistsAsync();

            return client;
        }
    }
}
