using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class DisciplinaryOpennessContentDTO
    {
        [JsonPropertyName("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("prosecutorId")]
        public string ProsecutorId { get; set; } = default!;
        [JsonPropertyName("complainantName")]
        public string ComplainantName { get; set; } = default!;
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("audienceStartDate")]
        public DateTime AudienceStartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [JsonPropertyName("audienceEndDate")]
        public DateTime AudienceEndDate { get; set; } = DateTime.UtcNow.AddDays(-5);
        [JsonPropertyName("participants")]
        public List<string> Participants { get; set; } = new();
        [JsonPropertyName("audiencePlace")]
        public string AudiencePlace { get; set; } = default!;
        [JsonPropertyName("imputedFacts")]
        public List<string> ImputedFacts { get; set; } = new();
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; } = default!;
    }
}
