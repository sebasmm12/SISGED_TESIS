using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.DossierDocument
{
    public class DossierDocumentInitialRequestResponse
    {
        public Entities.Dossier Dossier { get; set; }
        public Entities.InitialRequest InitialRequest{ get; set; }
    }
}
