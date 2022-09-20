using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class LegalAct
    {
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("contratos")]
        public List<Contract> Contracts { get; set; } = default!;
        [BsonElement("otorgantes")]
        public List<Grantor> Grantors { get; set; } = default!;
    }
}
