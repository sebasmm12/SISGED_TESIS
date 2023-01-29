using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class DocumentContentDTO
    {
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("codigo")]
        public string Code { get; set; } = default!;
    }
}
