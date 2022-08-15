using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Step
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("nombreexpediente")]
        public string DossierName { get; set; } = default!;
        [BsonElement("documentos")]
        public List<StepDocument> Documents { get; set; } = default!;
    }

    public class StepDocument
    {
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("pasos")]
        public List<DocumentStep> Steps { get; set; } = default!;
    }
    public class DocumentStep
    {
        [BsonElement("indice")]
        public int Index { get; set; }
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
        [BsonElement("fechainicio")]
        public DateTime? StartDate { get; set; }
        [BsonElement("fechafin")]
        public DateTime? EndDate { get; set; }
        [BsonElement("fechalimite")]
        public DateTime? DueDate { get; set; }
        [BsonElement("dias")]
        public int Days { get; set; }
        [BsonElement("subpaso")]
        public List<Substep> Substep { get; set; } = default!;

    }

    public class Substep
    {
        [BsonElement("indice")]
        public int Index { get; set; }
        [BsonElement("descripcion")]
        public string Description { get; set; } = default!;
    }
}
