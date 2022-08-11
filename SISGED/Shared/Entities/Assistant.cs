using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Assistant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
        [BsonElement("idexpediente")]
        public String DossierId { get; set; } = default!;
        [BsonElement("pasos")]
        public AssistantStep Steps { get; set; } = default!;
        [BsonElement("paso")]
        public Int32 Step { get; set; }
        [BsonElement("subpaso")]
        public Int32 Substep { get; set; }
        [BsonElement("tipodocumento")]
        public String DocumentType { get; set; } = default!;
    }

    public class AssistantStep
    {
        public String DossierName { get; set; } = default!;
        public List<DocumentStep> Documents { get; set; } = default!;
    }
}
