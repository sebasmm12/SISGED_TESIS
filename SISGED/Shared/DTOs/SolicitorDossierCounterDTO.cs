using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierCounterDTO
    {
        [BsonElement("total")]
        public int Total { get; set; }
    }
}
