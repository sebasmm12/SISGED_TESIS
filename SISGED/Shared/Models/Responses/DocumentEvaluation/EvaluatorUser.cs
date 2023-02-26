using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.DocumentEvaluation
{
    public class EvaluatorUser
    {
        [BsonElement("firstName")]
        public string FirstName { get; set; } = default!;
        [BsonElement("lastName")]
        public string LastName { get; set; } = default!;
        [BsonElement("image")]
        public string Image { get; set; } = default!;
    }
}
