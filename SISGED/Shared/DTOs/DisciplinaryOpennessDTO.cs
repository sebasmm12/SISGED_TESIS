using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.User;
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
        public ProsecutorUserInfoResponse Prosecutor { get; set; } = default!;
        public DisciplinaryOpennessDTO() {}
        public DisciplinaryOpennessDTO(Client client, AutocompletedSolicitorResponse solicitor, ProsecutorUserInfoResponse prosecutor)
        {
            Client = client;
            Solicitor = solicitor;
            Prosecutor = prosecutor;
        }
    }
}
