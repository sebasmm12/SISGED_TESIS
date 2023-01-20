using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierRequestDTO
    {
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;

        public SolicitorDossierRequestDTO() { }
        public SolicitorDossierRequestDTO(Client client, AutocompletedSolicitorResponse solicitor)
        {
            Client = client;
            Solicitor = solicitor;
        }
    }
}
