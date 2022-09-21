using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Requests.Documents
{
    public class GenerateDocumentRequest
    {
        public string PreviousDocumentId { get; set; } = default!;
        public string DocumentId { get; set; } = default!;
        public string DossierId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Sign { get; set; } = default!;
        public string GeneratedURL { get; set; } = default!;
    }
}
