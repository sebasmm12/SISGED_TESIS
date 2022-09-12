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
        public string SenderId { get; set; } = default!;
        [BsonElement("idreceptor")]
        public string ReceiverId { get; set; } = default!;

        public Process()
        {
            
        }

        public Process(string area, string senderId, string receiverId, DateTime? receiptDate = null,
                       DateTime? issuanceDate = null)
        {
            Area = area;
            SenderId = senderId;
            ReceiverId = receiverId;
            ReceiptDate = receiptDate ?? DateTime.UtcNow.AddHours(-5);
            IssuanceDate = issuanceDate ?? DateTime.UtcNow.AddHours(-5);

        }
    }
}
