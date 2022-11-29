using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.DossierDocument
{
    public class DossierDocumentInitialRequestResponse
    {
        public Entities.Dossier Dossier { get; set; } = default!;
        public Entities.InitialRequest InitialRequest { get; set; } = default!;

        public DossierDocumentInitialRequestResponse() { }

        public DossierDocumentInitialRequestResponse(Entities.Dossier dossier, InitialRequest initialRequest)
        {
            Dossier = dossier;
            InitialRequest = initialRequest;
        }
    }
}
