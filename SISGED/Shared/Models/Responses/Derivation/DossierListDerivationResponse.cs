using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Derivation
{
    public class DossierListDerivationResponse
    {
        [BsonElement("originArea")]
        public string OriginArea { get; set; } = default!;
        [BsonElement("targetArea")]
        public string TargetArea { get; set; } = default!;
        [BsonElement("senderUser")]
        public string SenderUser { get; set; } = default!;
        [BsonElement("senderImage")]
        public string SenderImage{ get; set; } = default!;
        [BsonElement("receiverUser")]
        public string ReceiverUser { get; set; } = default!;
        [BsonElement("receiverImage")]
        public string ReceiverImage { get; set; } = default!;
        [BsonElement("derivationDate")]
        public DateTime DerivationDate { get; set; }
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
    }
}
