using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Dossier
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("cliente")]
        public Cliente Client { get; set; } = default!;
        [BsonElement("fechainicio")]
        public DateTime StartDate { get; set; }
        [BsonElement("fechafin")]
        public DateTime? EndDate { get; set; }
        [BsonElement("documentos")]
        public List<DossierDocument> Documents { get; set; } = default!;
        [BsonElement("derivaciones")]
        public List<Derivation> Derivations { get; set; } = default!;
        [BsonElement("estado")]
        public string State { get; set; } = default!;
    }
}
