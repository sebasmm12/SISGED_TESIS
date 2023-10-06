using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Responses.Document
{
    public class SessionResolutionResponse : Entities.Document
    {
        public List<MediaRegisterDTO> URLAnnex { get; set; } = new();

        public SessionResolutionResponseContent Content { get; set; } = new();

        public SessionResolutionResponse(SessionResolutionResponseContent content, List<MediaRegisterDTO> urlAnnexes)
        {
            URLAnnex = urlAnnexes;
            Content = content;
        }

        public SessionResolutionResponse()
        {
            
        }
    }

    public class SessionResolutionResponseContent
    {
        public string Title { get; set; } = default!;

        public string PreviousDocumentId { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string ClientId { get; set; } = default!;

        public string SolicitorId { get; set; } = default!;
    }
}
