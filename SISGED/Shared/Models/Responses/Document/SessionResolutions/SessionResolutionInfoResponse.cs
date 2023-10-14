using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.SessionResolutions
{
    public class SessionResolutionInfoResponse : DocumentInfoResponse
    {
        [BsonElement("content")]
        public SessionResolutionContentInfo Content { get; set; } = default!;
    }

    public class SessionResolutionContentInfo : SessionResolutionInfo
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;

        [BsonElement("title")]
        public string Title { get; set; } = default!;

        [BsonElement("description")]
        public string Description { get; set; } = default!;
    }
}
