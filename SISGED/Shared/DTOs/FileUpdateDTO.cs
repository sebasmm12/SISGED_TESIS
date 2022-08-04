using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class FileUpdateDTO
    {
        public FileUpdateDTO(byte[] content, string extension, string containerName, string url)
        {
            Content = content;
            Extension = extension;
            ContainerName = containerName;
            Url = url;
        }

        public byte[] Content { get; set; } = default!;
        public string Extension { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}
