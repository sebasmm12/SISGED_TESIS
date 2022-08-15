using SISGED.Shared.Models.Responses.DossierTray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Tray
{
    public class InputOutputTrayResponse
    {
        public string Id { get; set; } = default!;
        public List<DossierTrayResponse> OutputDossier { get; set; }
          = new List<DossierTrayResponse>();
        public List<DossierTrayResponse> InputDossier { get; set; }
          = new List<DossierTrayResponse>();
    }
}
