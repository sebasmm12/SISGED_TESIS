using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.DisciplinaryOpenness
{
    public class DisciplinaryOpennessInfoResponse: DocumentInfoResponse
    {
        [BsonElement("content")]
        public DisciplinaryOpennessContentInfo Content { get; set; } = default!;
    }

    public class DisciplinaryOpennessContentInfo
    {
        [BsonElement("solicitor")]
        public Entities.Solicitor Solicitor { get; set; } = default!;
        [BsonElement("fiscalId")]
        public string FiscalId { get; set; } = default!;
        [BsonElement("complainantName")]
        public string ComplainantName { get; set; } = default!;
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
        [BsonElement("url")]
        public string Url { get; set; } = default!;
    }
}
