using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class InitialRequestContentDTO
    {
        [JsonPropertyName("descripcion")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("idtiposolicitud")]
        public string RequestTypeId { get; set; } = default!;
        [JsonPropertyName("tienenotario")]
        public bool HasSolicitor { get; set; }
        [JsonPropertyName("idnotario")]
        public string SolicitorId { get; set; } = default!;
    }
}
