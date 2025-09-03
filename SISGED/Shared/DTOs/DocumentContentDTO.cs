using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.DTOs
{
    public class DocumentContentDTO
    {
        [BsonElement("title")]
        public string Title { get; set; } = default!;

        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("generatedUrl")]
        public string GeneratedUrl { get; set; }
    }
}
