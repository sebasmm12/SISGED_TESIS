using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class DisciplinaryOpennessDTO
    {
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public DisciplinaryOpennessDTO() {}
        public DisciplinaryOpennessDTO(Client client, AutocompletedSolicitorResponse solicitor)
        {
            Client = client;
            Solicitor = solicitor;
        }
    }
}
