using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.BPNResult
{
    public class BPNResultInfoResponse: DocumentInfoResponse
    {
        [BsonElement("content")]
        public BPNResultContentInfo Content { get; set; } = default!;
    }
    
    public class BPNResultContentInfo
    {
        [BsonElement("totalSheets")]
        public int TotalSheets { get; set; }
        [BsonElement("cost")]
        public int Cost { get; set; }
        [BsonElement("status")]
        public string Status { get; set; } = default!;
        [BsonElement("publicDeed")]
        public Entities.PublicDeed PublicDeed { get; set; } = default!;
    }
   
}
