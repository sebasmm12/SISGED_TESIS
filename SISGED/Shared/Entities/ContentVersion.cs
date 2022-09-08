using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class ContentVersion
    {
        [BsonElement("version")]
        public int Version { get; set; } = default!;
        [BsonElement("fechamodificacion")]
        public DateTime ModificationDate { get; set; } = DateTime.Now;
        [BsonElement("url")]
        public string Url { get; set; } = default!;
    }
}
