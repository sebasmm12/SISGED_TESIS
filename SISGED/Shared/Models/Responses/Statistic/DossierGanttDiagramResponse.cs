using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Statistic
{
    public class DossierGanttDiagramResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string DossierType { get; set; } = default!;
        [BsonElement("documentId")]
        public string DocumentId { get; set; } = default!;
        [BsonElement("documentType")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("client")]
        public Client Client { get; set; } = default!;
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }
        [BsonElement("delayDate")]
        public DateTime DelayDate { get; set; }
        [BsonElement("state")]
        public string State { get; set; } = default!;

    }
}
