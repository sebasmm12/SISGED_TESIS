using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.DTOs;

namespace SISGED.Shared.Models.Responses.Document
{
    public class ComplaintRequestResponse : Entities.Document
    {
        public string ClientName { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public List<MediaRegisterDTO> URLAnnex { get; set; } = new();

        public ComplaintRequestResponseContent Content { get; set; } = new();

        public ComplaintRequestResponse(ComplaintRequestResponseContent content, List<MediaRegisterDTO> urlAnnexes)
        {
            Content = content;
            URLAnnex = urlAnnexes;
        }

        public ComplaintRequestResponse() { }

    }
    public class ComplaintRequestResponseContent
    {
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string SolicitorId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ComplaintType { get; set; } = default!;
        public DateTime DeliveryDate { get; set; }

    }
}
