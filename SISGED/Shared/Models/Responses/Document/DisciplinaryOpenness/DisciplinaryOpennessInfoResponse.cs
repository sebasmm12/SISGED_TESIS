using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness
{
    public class DisciplinaryOpennessInfoResponse: DocumentInfoResponse
    {
        [BsonElement("content")]
        public DisciplinaryOpennessContentInfo Content { get; set; } = default!;
    }

    public class DisciplinaryOpennessContentInfo : DisciplinaryOpennessRequestInfo
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("audienceStartDate")]
        public DateTime AudienceStartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("audienceEndDate")]
        public DateTime AudienceEndDate { get; set; } = DateTime.UtcNow.AddDays(-5);
        [BsonElement("participants")]
        public List<string> Participants { get; set; } = new();
        [BsonElement("audiencePlace")]
        public string AudiencePlace { get; set; } = default!;
        [BsonElement("imputedFacts")]
        public List<string> ImputedFacts { get; set; } = new();
    }
}
