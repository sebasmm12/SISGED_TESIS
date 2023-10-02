using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class ComplaintRequestContentDTO
    {
        public string Description { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string SolicitorId { get; set; } = default!;

        public string ClientId { get; set; } = default!;

        public string ComplaintType { get; set; } = default!;

        public DateTime DeliveryDate { get; set; }

    }
}
