using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierShipmentContentDTO
    {

        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("solicitorDossiers")]
        public List<string>? SolicitorDossiers { get; set; }
    }
}
