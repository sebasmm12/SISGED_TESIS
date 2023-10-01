using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DocumentResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("contentsHistory")]
        public List<ContentVersion> ContentsHistory { get; set; } = default!;
        [BsonElement("processesHistory")]
        public List<Process> ProcessesHistory { get; set; } = default!;
        [BsonElement("content")]
        public object Content { get; set; } = default!;
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("evaluations")]
        public List<Entities.DocumentEvaluation> Evaluations { get; set; } = default!;
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; } = default!;
        [BsonElement("urlAnnex")]
        public List<string> UrlAnnex { get; set; } = default!;
    }
}
