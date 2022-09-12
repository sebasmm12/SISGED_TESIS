using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class DocumentTray
    {
        [BsonElement("idexpediente")]
        public string DossierId { get; set; } = default!;
        [BsonElement("iddocumento")]
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
