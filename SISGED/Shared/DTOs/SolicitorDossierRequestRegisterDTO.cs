using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Solicitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierRequestRegisterDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        //public Client Client { get; set; } = = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
    }
}
