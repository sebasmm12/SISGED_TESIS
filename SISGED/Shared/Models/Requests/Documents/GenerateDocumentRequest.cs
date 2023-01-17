using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Requests.Documents
{
    public class GenerateDocumentRequest
    {
        public GenerateDocumentRequest(string documentId, string dossierId, string code)
        {
            DocumentId = documentId;
            DossierId = dossierId;
            Code = code;
        }
        
        public GenerateDocumentRequest() {  }

        public string PreviousDocumentId { get; set; } = default!;
        public string DocumentId { get; set; } = default!;
        public string DossierId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Code { get; set; } = default!;
        public MediaRegisterDTO Sign { get; set; } = default!;
        public MediaRegisterDTO GeneratedURL { get; set; } = default!;

       
    }
}
