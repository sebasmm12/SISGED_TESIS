using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class DocumentEvaluation
    {
        [BsonElement("userEvaluator")]
        public string UserEvaluator { get; set; } = default!;
        [BsonElement("isApproved")]
        public bool IsApproved { get; set; }
        [BsonElement("comment")]
        public string? Comment { get; set; } = default!;
        [BsonElement("evaluationDate")]
        public DateTime EvaluationDate { get; set; } = default!;

        public DocumentEvaluation()
        {

        }

        public DocumentEvaluation(string userEvaluator, bool isApproved, string? comment, DateTime evaluationDate)
        {
            UserEvaluator = userEvaluator;
            IsApproved = isApproved;
            Comment = comment;
            EvaluationDate = evaluationDate;
        }
    }
}
