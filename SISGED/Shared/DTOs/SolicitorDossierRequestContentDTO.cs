using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierRequestContentDTO
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("issueDate")]
        public DateTime IssueDate { get; set; }
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; } = default!;
    }
}
