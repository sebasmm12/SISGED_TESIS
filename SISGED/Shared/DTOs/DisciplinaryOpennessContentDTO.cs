using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class DisciplinaryOpennessContentDTO
    {
        [JsonPropertyName("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("idfiscal")]
        public string ProsecutorId { get; set; } = default!;
        [JsonPropertyName("nombredenunciante")]
        public string ComplainantName { get; set; } = default!;
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("descripcion")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("fechainicioaudiencia")]
        public DateTime AudienceStartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [JsonPropertyName("fechafinaudiencia")]
        public DateTime AudienceEndDate { get; set; } = DateTime.UtcNow.AddDays(-5);
        [JsonPropertyName("participantes")]
        public List<string> Participants { get; set; } = new();
        [JsonPropertyName("lugaraudiencia")]
        public string AudiencePlace { get; set; } = default!;
        [JsonPropertyName("hechosimputados")]
        public List<string> ImputedFacts { get; set; } = new();
        [JsonPropertyName("iddenunciante")]
        public string ClientId { get; set; } = default!;
    }
}
