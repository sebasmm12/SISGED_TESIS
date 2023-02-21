using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.UserDocument
{
    public class UserDocumentResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("historialcontenido")]
        public List<ContentVersion> ContentsHistory { get; set; } = default!;
        [BsonElement("historialproceso")]
        public List<Process> ProcessesHistory { get; set; } = default!;
        [BsonElement("contenido")]
        public object Content { get; set; } = default!;
        [BsonElement("estado")]
        public string State { get; set; } = default!;
        [BsonElement("evaluaciones")]
        public List<DocumentEvaluation> Evaluations { get; set; } = default!;
        [BsonElement("fechacreacion")]
        public DateTime CreationDate { get; set; } = default!;
        [BsonElement("urlanexo")]
        public List<string> UrlAnnex { get; set; } = default!;

        [BsonElement("cliente")]
        public Client Client { get; set; } = default!;
        [BsonElement("tipoExpediente")]
        public string DossierType { get; set; } = default!;

    }
}
