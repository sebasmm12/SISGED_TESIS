using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Process
    {
        [BsonElement("area")]
        public string? Area { get; set; } = default!;
        [BsonElement("receiptDate")]
        public DateTime ReceiptDate { get; set; }
        [BsonElement("issuanceDate")]
        public DateTime IssuanceDate { get; set; }
        [BsonElement("senderId")]
        public string SenderId { get; set; } = default!;
        [BsonElement("receiverId")]
        public string ReceiverId { get; set; } = default!;
        [BsonElement("state")]
        public string State { get; set; } = default!;

        public Process()
        {
            
        }

        public Process(string senderId, string receiverId, string state, string? area = null, DateTime? receiptDate = null,
                       DateTime? issuanceDate = null)
        {
            Area = area;
            SenderId = senderId;
            ReceiverId = receiverId;
            State = state;
            ReceiptDate = receiptDate ?? DateTime.UtcNow.AddHours(-5);
            IssuanceDate = issuanceDate ?? DateTime.UtcNow.AddHours(-5);

        }
    }
}
