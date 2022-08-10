using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class DossierDocument
    {
        [BsonElement("indice")]
        public int Index { get; set; }
        [BsonElement("iddocumento")]
        public string DocumentId { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("fechacreacion")]
        public DateTime CreationDate { get; set; }
        [BsonElement("fechaexceso")]
        public DateTime ExcessDate { get; set; }
        [BsonElement("fechademora")]
        public DateTime? DelayDate { get; set; }
    }
}
