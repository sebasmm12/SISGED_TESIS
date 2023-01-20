using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierRequestContentDTO
    {
        [JsonPropertyName("titulo")]
        public string Title { get; set; } = default!;
        [JsonPropertyName("descripcion")]
        public string Description { get; set; } = default!;
        [JsonPropertyName("idnotario")]
        public string SolicitorId { get; set; } = default!;
        [JsonPropertyName("fechaemision")]
        public DateTime IssueDate { get; set; }
        [JsonPropertyName("iddenunciante")]
        public string ClientId { get; set; } = default!;
    }
}
