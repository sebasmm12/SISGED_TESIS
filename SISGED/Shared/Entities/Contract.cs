using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Contract
    {
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("url")]
        public string Url { get; set; } = default!;
    }
}
