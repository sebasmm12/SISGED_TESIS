namespace SISGED.Shared.DTOs
{
    public class MediaRegisterDTO
    {
        public MediaRegisterDTO(string content, string extension, string name)
        {
            Content = content;
            Extension = extension;
            Name = name;
        }

        public string Content { get; set; } = default!;
        public string Extension { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
