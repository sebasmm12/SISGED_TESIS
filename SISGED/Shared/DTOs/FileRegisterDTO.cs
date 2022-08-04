using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class FileRegisterDTO
    {
        public FileRegisterDTO(byte[] content, string extension, string containerName)
        {
            Content = content;
            Extension = extension;
            ContainerName = containerName;
        }

        public byte[] Content { get; set; } = default!;
        public string Extension { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
    }
}
