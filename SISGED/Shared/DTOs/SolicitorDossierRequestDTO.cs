using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;;

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
