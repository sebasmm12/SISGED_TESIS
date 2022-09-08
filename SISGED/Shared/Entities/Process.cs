using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Process
    {
        [BsonElement("indice")]
        public int Index { get; set; }
        [BsonElement("area")]
        public string Area { get; set; } = default!;
        [BsonElement("fecharecepcion")]
        public DateTime ReceiptDate { get; set; }
        [BsonElement("fechaemision")]
        public DateTime IssuanceDate { get; set; }
        [BsonElement("idemisor")]
        public string IssueId { get; set; } = default!;
        [BsonElement("idreceptor")]
        public string ReceiverId { get; set; } = default!;
    }
}
