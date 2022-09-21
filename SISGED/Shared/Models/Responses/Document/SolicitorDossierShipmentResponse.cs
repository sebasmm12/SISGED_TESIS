using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class SolicitorDossierShipmentResponse : Entities.Document
    {
        public SolicitorDossierShipmentResponseContent Content { get; set; } = new SolicitorDossierShipmentResponseContent();
    }
    public class SolicitorDossierShipmentResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Entities.Solicitor SolicitorId { get; set; } = new Entities.Solicitor();

        public List<string> URLAnnex { get; set; } = new List<string>();
    }   
}
