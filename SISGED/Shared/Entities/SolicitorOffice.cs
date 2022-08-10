using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class SolicitorOffice
    {
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("ruc")]
        public string RUC { get; set; } = default!;
        [BsonElement("ubicacion")]
        public string Address { get; set; } = default!;
        [BsonElement("provincia")]
        public string Province { get; set; } = default!;
        [BsonElement("distrito")]
        public string District { get; set; } = default!;
    }
}
