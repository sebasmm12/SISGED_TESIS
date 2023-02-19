using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.User;

namespace SISGED.Shared.DTOs
{
    public class DisciplinaryOpennessDTO
    {
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public DisciplinaryOpennessDTO() { }
        public DisciplinaryOpennessDTO(Client client, AutocompletedSolicitorResponse solicitor)
        {
            Client = client;
            Solicitor = solicitor;
        }
    }
}
