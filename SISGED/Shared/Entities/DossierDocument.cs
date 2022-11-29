using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class DossierDocument
    {
        public DossierDocument() {  }

        public DossierDocument(int index, string documentId, string type, DateTime excessDate)
        {
            Index = index;
            DocumentId = documentId;
            Type = type;
            ExcessDate = excessDate;
        }

        [BsonElement("indice")]
        public int Index { get; set; }
        [BsonElement("iddocumento")]
        public string DocumentId { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("fechacreacion")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("fechaexceso")]
        public DateTime ExcessDate { get; set; }
        [BsonElement("fechademora")]
        public DateTime? DelayDate { get; set; }
    }
}
