using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.DocumentEvaluation
{
    public class DocumentEvaluationInfo
    {
        [BsonElement("evaluatorUser")]
        public EvaluatorUser EvaluatorUser { get; set; } = default!;
        [BsonElement("isApproved")]
        public bool IsApproved { get; set; }
        [BsonElement("comment")]
        public string? Comment { get; set; } = default!;
        [BsonElement("evaluationDate")]
        public DateTime EvaluationDate { get; set; } = default!;
    }
}
