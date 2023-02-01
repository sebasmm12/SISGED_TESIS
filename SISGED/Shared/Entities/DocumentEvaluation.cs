using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class DocumentEvaluation
    {
        [BsonElement("usuarioevaluador")]
        public string UserEvaluator { get; set; } = default!;
        [BsonElement("estaaprobado")]
        public bool IsApproved { get; set; }
        [BsonElement("observacion")]
        public string? Comment { get; set; } = default!;
        [BsonElement("fechaevaluacion")]
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
