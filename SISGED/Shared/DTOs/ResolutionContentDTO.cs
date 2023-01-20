using System.Text.Json.Serialization;


namespace SISGED.Shared.DTOs
{
    public class ResolutionContentDTO
    {
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("descripcion")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("fechainicioaudiencia")]
        public DateTime AudienceStartDate { get; set; }
        [JsonPropertyName("fechafinaudiencia")]
        public DateTime AudienceEndDate { get; set; }
        [JsonPropertyName("participantes")]
        public List<string> Participants { get; set; } = new();
        [JsonPropertyName("sancion")]
        public string Sanction { get; set; } = default!;
        [JsonPropertyName("iddenunciante")]
        public string ClientId { get; set; } = default!;
        [JsonPropertyName("idnotario")]
        public string SolicitorId { get; set; } = default!;
    }
}
