using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class DocumentTray
    {
        [BsonElement("idexpediente")]
        public string ReportId { get; set; } = default!;
        [BsonElement("iddocumento")]
        public string DocumentId { get; set; } = default!;
    }
}
