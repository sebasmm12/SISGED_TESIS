using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.Resolution
{
    public class ResolutionInfoResponse: DocumentInfoResponse
    {
        [BsonElement("content")]
        public ResolutionContentInfo Content { get; set; } = default!;
    }

    public class ResolutionContentInfo : ResolutionInfo
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("audienceStartDate")]
        public DateTime AudienceStartDate { get; set; }
        [BsonElement("audienceEndDate")]
        public DateTime AudienceEndDate { get; set; }
        [BsonElement("participants")]
        public List<string> Participants { get; set; } = new();
        [BsonElement("sanction")]
        public string Sanction { get; set; } = default!;
    }
}
