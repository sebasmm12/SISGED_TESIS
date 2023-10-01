using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class ResolutionContentDTO
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("audienceStartDate")]
        public DateTime AudienceStartDate { get; set; }
        [JsonPropertyName("audienceEndDate")]
        public DateTime AudienceEndDate { get; set; }
        [JsonPropertyName("participants")]
        public List<string> Participants { get; set; } = new();
        [JsonPropertyName("sanction")]
        public string Sanction { get; set; } = default!;
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; } = default!;
        [JsonPropertyName("solicitorId")]
        public string SolicitorId { get; set; } = default!;
    }
}
