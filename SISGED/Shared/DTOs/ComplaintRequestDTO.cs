using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class ComplaintRequestDTO : ComplaintDocumentDTO
    {
        public DocumentTypeInfoResponse ComplaintType { get; set; } = default!;

        public ComplaintRequestDTO() {  }

        public ComplaintRequestDTO(Client client, AutocompletedSolicitorResponse solicitor, DocumentTypeInfoResponse complaintType) : base(client, solicitor)
        {
            ComplaintType = complaintType;
        }
    }
}
