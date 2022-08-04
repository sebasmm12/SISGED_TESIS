using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Tray
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("usuario")]
        public string User { get; set; } = default!;
        [BsonElement("bandejaentrada")]
        public List<DocumentTray> InputTray { get; set; } = new List<DocumentTray>();
        [BsonElement("bandejasalida")]
        public List<DocumentTray> OutputTray { get; set; } = new List<DocumentTray>();
        
        public Tray(string type, string userId)
        {
            Type = type;
            User = userId;
        }
    }
}
