namespace SISGED.Shared.DTOs
{
    public class FileStreamRegisterDTO
    {
        public FileStreamRegisterDTO(string? image, string extension, string containerName)
        {
            Image = image;
            Extension = extension;
            ContainerName = containerName;
        }

        public string? Image { get; set; }
        public string Extension { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
    }
}
