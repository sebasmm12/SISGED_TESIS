using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Steps
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; } = default!;
        [BsonElement("nombreexpediente")]
        public String DossierName { get; set; } = default!;
        [BsonElement("documentos")]
        public List<DocumentStep> Documents { get; set; } = default!;
    }

    public class DocumentStep
    {
        public string Type { get; set; } = default!;
        public List<Step> Steps { get; set; } = default!;
    }
    public class Step
    {

        public Int32 Index { get; set; }
        public String Name { get; set; } = default!;
        public String Description { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public Int32 Days { get; set; }
        public List<Substep> Substep { get; set; } = default!;

    }

    public class Substep
    {
        public Int32 Index { get; set; }
        public String Description { get; set; } = default!;
    }
}
