using SISGED.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class SolicitorDossierShipmentResponse : Entities.Document
    {
        public SolicitorDossierShipmentResponseContent Content { get; set; } = new();
        public List<MediaRegisterDTO> URLAnnex { get; set; } = new();

        public SolicitorDossierShipmentResponse(SolicitorDossierShipmentResponseContent content, List<MediaRegisterDTO> urlAnnexes)
        {
            Content = content;
            URLAnnex = urlAnnexes;
        }

        public SolicitorDossierShipmentResponse() { }

    }
    public class SolicitorDossierShipmentResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string SolicitorId { get; set; } = default!;
        public List<string> SolicitorDossiers { get; set; } = default!;
    }   
}
