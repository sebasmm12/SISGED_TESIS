using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.DocumentProcess
{
    public class DocumentProcessInfo
    {
        [BsonElement("area")]
        public string Area { get; set; } = default!;
        [BsonElement("receiptDate")]
        public DateTime ReceiptDate { get; set; } = default!;
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("senderUser")]
        public ProcessUser SenderUser { get; set; } = default!;
        [BsonElement("receiverUser")]
        public ProcessUser ReceiverUser { get; set; } = default!;
    }
}
