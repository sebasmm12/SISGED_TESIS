using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class DictumContentDTO
    {
        [JsonPropertyName("complaintId")]
        public string ComplaintId { get; set; } = default!;
        [JsonPropertyName("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("observations")]
        public List<string> Observations { get; set; } = new();
        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; } = default!;
        [JsonPropertyName("recommendations")]
        public List<string> Recommendations { get; set; } = new();
    }
}
