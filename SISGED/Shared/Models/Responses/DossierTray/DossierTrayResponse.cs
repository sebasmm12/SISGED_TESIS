using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.DossierTray
{
    public class DossierTrayResponse
    {
        public string DossierId { get; set; } = default!;
        public Client Client { get; set; }
        public DocumentResponse Document { get; set; }
        public List<DocumentResponse> DocumentObjects { get; set; }
        public string Type { get; set; } = default!;
    }
}
