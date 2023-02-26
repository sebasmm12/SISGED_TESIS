using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.Dictum
{
    public class DictumInfoResponse : DocumentInfoResponse
    {
        [BsonElement("content")]
        public DictumContentInfo Content { get; set; } = default!;
    }

    public class DictumContentInfo : ComplaintRequestInfo
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("observations")]
        public List<string> Observations { get; set; } = new();
        [BsonElement("conclusion")]
        public string Conclusion { get; set; } = default!;
        [BsonElement("recommendations")]
        public List<string> Recommendations { get; set; } = new();
    }
}
