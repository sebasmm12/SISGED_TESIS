using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Statistic
{
    public class ExpiredDocumentsResponse
    {
        [BsonElement("documentType")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("quantity")]
        public int ExpiredDocuments { get; set; }
    }
}
