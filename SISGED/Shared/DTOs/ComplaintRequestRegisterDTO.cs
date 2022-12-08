using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class ComplaintRequestRegisterDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public DocumentTypeInfoResponse ComplaintType { get; set; } = default!;
    }
}
