using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.DTOs
{
    public class UserDocumentCounterDTO
    {
        [BsonElement("total")]
        public int Total { get; set; }
    }
}
