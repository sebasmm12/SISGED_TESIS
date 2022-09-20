

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DocumentInfoResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("contentsHistory")]
        public List<ContentVersion> ContentsHistory { get; set; } = default!;
        [BsonElement("processesHistory")]
        public List<Process> ProcessesHistory { get; set; } = default!;
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("attachedUrls")]
        public List<string> AttachedUrls { get; set; } = new();
    }
}
