using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DictumResponse : Entities.Document
    {
        public List<MediaRegisterDTO> URLAnnex { get; set; } = new();
        public DictumResponseContent Content { get; set; } = new DictumResponseContent();

        public DictumResponse(DictumResponseContent content, List<MediaRegisterDTO> urlAnnexes) 
        {
            Content = content;
            URLAnnex = urlAnnexes;
        }

        public DictumResponse() {  }
    }

    public class DictumResponseContent
    {
        public string ComplaintId { get; set; } = default!;
        public string SolicitorId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public List<string> Observations { get; set; } = new();
        public string Conclusion { get; set; } = default!;
        public List<string> Recommendations { get; set; } = new();
    }
}
