using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Responses.Document
{
    public class InitialRequestResponse : Entities.Document
    {
        public string ClientName { get; set; } = default!;
        public string ClientLastName { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public List<MediaRegisterDTO> URLAnnex { get; set; } = new();
        public string ClientId { get; set; } = default!;
        public InitialRequestResponseContent Content { get; set; } = new InitialRequestResponseContent();

        public InitialRequestResponse(InitialRequestResponseContent content, List<MediaRegisterDTO> urlAnnex, string clientName, string clientLastName, 
            string documentType, string documentNumber, string clientId)
        {
            Content = content;
            URLAnnex = urlAnnex;
            ClientName = clientName;
            ClientLastName = clientLastName;
            DocumentType = documentType;
            DocumentNumber = documentNumber;
            ClientId = clientId;
        }

        public InitialRequestResponse() {  }
    }

    public class InitialRequestResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string RequestTypeId { get; set; } = default!;
        public bool HasSolicitor { get; set; }
        public string SolicitorId { get; set; } = default!;
    }
}
