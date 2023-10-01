using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Derivation
    {
        public Derivation(string originArea, string targetArea, string senderUser, string state, string type)
        {
            OriginArea = originArea;
            TargetArea = targetArea;
            SenderUser = senderUser;
            State = state;
            Type = type;
            ReceiverUser = string.Empty;
        }

        public Derivation() { }

        [BsonElement("originArea")]
        public string OriginArea { get; set; } = default!;
        [BsonElement("targetArea")]
        public string TargetArea { get; set; } = default!;
        [BsonElement("senderUser")]
        public string SenderUser { get; set; } = default!;
        [BsonElement("receiverUser")]
        public string ReceiverUser { get; set; } = default!;
        [BsonElement("derivationDate")]
        public DateTime DerivationDate { get; set; } 
        [BsonElement("state")]
        public string State { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
    }
}
