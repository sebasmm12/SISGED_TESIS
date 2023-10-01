using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class PublicDeed
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("judicialOfficeDirection")]
        public string JudicialOfficeDirection { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("solicitorId")]
        public string SolicitorId { get; set; } = default!;
        [BsonElement("legalActs")]
        public List<LegalAct> LegalActs { get; set; } = default!;
        [BsonElement("publicDeedDate")]
        public DateTime PublicDeedDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("state")]
        public string State { get; set; } = default!;
    }
}
