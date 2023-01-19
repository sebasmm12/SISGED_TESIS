using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class ComplaintRequestDTO
    {
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public DocumentTypeInfoResponse ComplaintType { get; set; } = default!;

        public ComplaintRequestDTO() {  }

        public ComplaintRequestDTO(Client client, AutocompletedSolicitorResponse solicitor, DocumentTypeInfoResponse complaintType)
        {
            Client = client;
            Solicitor = solicitor;
            ComplaintType = complaintType;
        }
    }
}
