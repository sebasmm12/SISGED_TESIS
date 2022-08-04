using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class FileEliminationDTO
    {
        public FileEliminationDTO(string url, string containerName)
        {
            Url = url;
            ContainerName = containerName;
        }

        public string Url { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
    }
}
