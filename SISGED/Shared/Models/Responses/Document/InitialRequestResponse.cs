using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Responses.Document
{
    public class InitialRequestResponse : Entities.Document
    {
        public string ClientName { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public List<MediaRegisterDTO> URLAnnex { get; set; } = new();
        public Entities.User ClientId { get; set; } = new Entities.User();
        public InitialRequestResponseContent Content { get; set; } = new InitialRequestResponseContent();

        public InitialRequestResponse(InitialRequestResponseContent content, List<MediaRegisterDTO> urlAnnex)
        {
            Content = content;
            URLAnnex = urlAnnex;
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
