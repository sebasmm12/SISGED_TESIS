using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class UserRequestRegisterDTO
    {
        public DocumentTypeInfoResponse DocumentType { get; set; } = default!;
        public string Title { get; set; } = default!;
        public bool HasSolicitor { get; set; }
        public SolicitorInfoResponse Solicitor { get; set; } = default!;
        public string Description { get; set; } = default!;
        
    }
}
