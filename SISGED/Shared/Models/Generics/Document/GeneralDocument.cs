using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Generics.Document
{
    public class GeneralDocument
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
        [BsonElement("attachedUrls")]
        public List<string> AttachedUrls { get; set; } = new();
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("evaluations")]
        public List<DocumentEvaluation> Evaluations { get; set; } = new();
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }
        [BsonElement("creationUserId")]
        public string CreationUserId { get; set; } = default!;
        [BsonElement("endDate")]
        public DateTime? EndDate { get; set; }
        [BsonElement("dueDate")]
        public DateTime DueDate { get; set; }
    }
}
