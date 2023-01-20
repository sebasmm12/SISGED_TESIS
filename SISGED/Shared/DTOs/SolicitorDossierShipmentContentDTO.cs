using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierShipmentContentDTO
    {

        [JsonPropertyName("descripcion")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("expedientes")]
        public List<string>? SolicitorDossiers { get; set; }
    }
}
