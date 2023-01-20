using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class ComplaintDocumentDTO
    {
        public ComplaintDocumentDTO(Client client, AutocompletedSolicitorResponse solicitor)
        {
            Client = client;
            Solicitor = solicitor;
        }

        public ComplaintDocumentDTO() { }

        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
    }
}
