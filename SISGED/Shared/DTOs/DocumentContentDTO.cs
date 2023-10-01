using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class DocumentContentDTO
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;
    }
}
