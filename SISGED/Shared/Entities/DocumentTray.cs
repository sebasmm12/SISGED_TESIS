using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class DocumentTray
    {
        [BsonElement("dossierId")]
        public string DossierId { get; set; } = default!;
        [BsonElement("documentId")]
        public string DocumentId { get; set; } = default!;

        public DocumentTray()
        {
            
        }

        public DocumentTray(string dossierId, string documentId)
        {
            DossierId = dossierId;
            DocumentId = documentId;
        }
    }
}
