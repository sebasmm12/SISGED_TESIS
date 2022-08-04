namespace SISGED.Shared.DTOs
{
    public class FileStreamUpdateDTO
    {
        public FileStreamUpdateDTO(string? newImage, string? currentImage, string extension, string containerName)
        {
            NewImage = newImage;
            CurrentImage = currentImage;
            Extension = extension;
            ContainerName = containerName;
        }

        public string? NewImage { get; set; }
        public string? CurrentImage { get; set; }
        public string Extension { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
    }
}
