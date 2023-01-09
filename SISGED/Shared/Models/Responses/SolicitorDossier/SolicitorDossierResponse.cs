using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SISGED.Shared.Models.Responses.SolicitorDossier
{
    public class SolicitorDossierResponse
    {
        [BsonElement("nombredocumento")]
        public string DocumentName { get; set; } = default!;
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
    }
}
