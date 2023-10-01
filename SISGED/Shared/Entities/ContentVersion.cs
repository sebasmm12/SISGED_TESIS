using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class ContentVersion
    {
        [BsonElement("version")]
        public int Version { get; set; } = default!;

        [BsonElement("modificationDate")]
        public DateTime ModificationDate { get; set; } = DateTime.UtcNow.AddHours(-5);

        [BsonElement("url")]
        public string Url { get; set; } = default!;

        public ContentVersion() {  }

        public ContentVersion(int version, string url)
        {
            Version = version;
            Url = url;
        }
    }
}
