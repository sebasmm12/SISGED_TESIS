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

        public DossierDocument(string documentId)
        {
           DocumentId = documentId;
        }

        [BsonElement("index")]
        public int Index { get; set; }
        [BsonElement("documentId")]
        public string DocumentId { get; set; } = default!;
        [BsonElement("type")]
        public string Type { get; set; } = default!;
        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("excessDate")]
        public DateTime ExcessDate { get; set; }
        [BsonElement("delayDate")]
        public DateTime? DelayDate { get; set; }
    }
}
