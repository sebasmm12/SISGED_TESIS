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

        [BsonElement("areaprocedencia")]
        public string OriginArea { get; set; } = default!;
        [BsonElement("areadestino")]
        public string TargetArea { get; set; } = default!;
        [BsonElement("usuarioemisor")]
        public string SenderUser { get; set; } = default!;
        [BsonElement("usuarioreceptor")]
        public string ReceiverUser { get; set; } = default!;
        [BsonElement("fechaderivacion")]
        public DateTime DerivationDate { get; set; } 
        [BsonElement("estado")]
        public string State { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
    }
}
