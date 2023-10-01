using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class ComplaintRequestContentDTO
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; } = default!;
        [JsonPropertyName("complaintType")]
        public string ComplaintType { get; set; } = default!;
        [JsonPropertyName("deliveryDate")]
        public DateTime DeliveryDate { get; set; }

    }
}
