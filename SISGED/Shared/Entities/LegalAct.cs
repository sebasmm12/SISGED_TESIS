using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class LegalAct
    {
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("contracts")]
        public List<Contract> Contracts { get; set; } = default!;
        [BsonElement("grantors")]
        public List<Grantor> Grantors { get; set; } = default!;
    }
}
