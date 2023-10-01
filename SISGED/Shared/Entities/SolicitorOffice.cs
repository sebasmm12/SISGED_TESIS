using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class SolicitorOffice
    {
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("ruc")]
        public string RUC { get; set; } = default!;
        [BsonElement("address")]
        public string Address { get; set; } = default!;
        [BsonElement("province")]
        public string Province { get; set; } = default!;
        [BsonElement("district")]
        public string District { get; set; } = default!;
    }
}
