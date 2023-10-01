using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Requests.Documents
{
    public class DocumentRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Type { get; set; }
        public List<Entities.ContentVersion> ContentHistory { get; set; } = new();
        public List<Entities.Process> ProcessHistory { get; set; } = new();
        public object Content { get; set; }
        public object State { get; set; }
        public object Evaluation { get; set; }
        public DateTime CreationDate { get; set; }
        public List<string> URLAnnex { get; set; } = new();
    }
}
