using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Generics.Document
{
    public class DocumentInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("contentsHistory")]
        public List<ContentVersion> ContentsHistory { get; set; } = new();
        [BsonElement("processesHistory")]
        public List<Process> ProcessesHistory { get; set; } = new();
        [BsonElement("content")]
        public object Content { get; set; } = default!;
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("evaluations")]
        public List<DocumentEvaluation> Evaluations { get; set; } = default!;
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }
        [BsonElement("attachedUrls")]
        public List<string> AttachedUrls { get; set; } = new();
    }
}
