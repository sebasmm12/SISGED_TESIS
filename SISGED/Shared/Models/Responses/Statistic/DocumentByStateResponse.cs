using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Statistic
{
    public class DocumentByStateResponse
    {
        [BsonElement("documentType")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("expired")]
        public int ExpiredDocuments { get; set; }
        [BsonElement("processed")]
        public int ProcessedDocuments { get; set; }
        [BsonElement("pending")]
        public int PendingDocuments { get; set; }
    }
}
