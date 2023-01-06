using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierYearInfoDTO
    {
        [BsonElement("years")]
        public IEnumerable<int> Years { get; set; } = default!;
    }
}
