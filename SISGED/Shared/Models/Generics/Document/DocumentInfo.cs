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
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("historialcontenido")]
        public List<ContentVersion> ContentsHistory { get; set; } = new();
        [BsonElement("historialproceso")]
        public List<Process> ProcessesHistory { get; set; } = new();
        [BsonElement("contenido")]
        public object Content { get; set; } = default!;
        [BsonElement("estado")]
        public string State { get; set; } = default!;
        [BsonElement("evaluacion")]
        public object? Evaluation { get; set; }
        [BsonElement("fechacreacion")]
        public DateTime CreationDate { get; set; }
        [BsonElement("urlanexo")]
        public List<string> AttachedUrls { get; set; } = new();
    }
}
