using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class ComplaintRequestContentDTO
    {
        [JsonPropertyName("descripcion")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("idcliente")]
        public string ClientId { get; set; } = default!;
        [JsonPropertyName("tipoDenuncia")]
        public string ComplaintType { get; set; } = default!;
        [JsonPropertyName("fechaentrega")]
        public DateTime DeliveryDate { get; set; }

    }
}
