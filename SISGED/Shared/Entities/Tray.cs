using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Tray
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("user")]
        public string User { get; set; } = default!;
        [BsonElement("inputTray")]
        public List<DocumentTray> InputTray { get; set; } = new();
        [BsonElement("outputTray")]
        public List<DocumentTray> OutputTray { get; set; } = new();
        
        public Tray(string type, string userId)
        {
            Type = type;
            User = userId;
        }
    }
}
