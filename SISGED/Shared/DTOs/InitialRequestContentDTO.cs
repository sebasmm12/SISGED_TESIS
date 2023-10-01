using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class InitialRequestContentDTO
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("requestTypeId")]
        public string RequestTypeId { get; set; } = default!;
        [JsonPropertyName("hasSolicitor")]
        public bool HasSolicitor { get; set; }
        [JsonPropertyName("solicitorId")]
        public string SolicitorId { get; set; } = default!;
    }
}
