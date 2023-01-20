using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class DictumContentDTO
    {
        [JsonPropertyName("iddenunciante")]
        public string ComplaintId { get; set; } = default!;
        [JsonPropertyName("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("observaciones")]
        public List<string> Observations { get; set; } = new();
        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; } = default!;
        [JsonPropertyName("recomendaciones")]
        public List<string> Recommendations { get; set; } = new();
    }
}
