using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Dossier
{
    public class SolicitorDossierRequestResponse : Entities.Document
    {
        public SolicitorDossierRequestResponseContent Content { get; set; } = new SolicitorDossierRequestResponseContent();
    }

    public class SolicitorDossierRequestResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DateIssue { get; set; } = default!;
        public Entities.Solicitor SolicitorId { get; set; } = default!;
        public List<string> URLAnnex { get; set; } = new List<string>();
    }
}
