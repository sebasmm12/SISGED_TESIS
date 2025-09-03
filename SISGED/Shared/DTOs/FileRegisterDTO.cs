namespace SISGED.Shared.DTOs;

public class FileRegisterDTO
{
    public FileRegisterDTO(byte[] content, string extension, string containerName, string name = "")
    {
        Content = content;
        Extension = extension;
        ContainerName = containerName;
        Name = name;
    }

    public byte[] Content { get; set; }

    public string Extension { get; set; }

    public string ContainerName { get; set; }

    public string Name { get; set; }
}