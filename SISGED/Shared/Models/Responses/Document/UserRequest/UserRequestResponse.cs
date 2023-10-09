using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.UserRequest
{
    public class UserRequestResponse
    {
        [BsonElement("documentUrl")]
        public string? DocumentUrl { get; set; } = default!;

        [BsonElement("type")] 
        public string Type { get; set; } = default!;

        [BsonElement("initDate")]
        public DateTime InitDate { get; set; }

        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }

        [BsonElement("state")] 
        public string State { get; set; } = default!;

        [BsonElement("initialDocument")] 
        public UserRequestDocumentResponse InitialDocument { get; set; } = default!;
    }
}
