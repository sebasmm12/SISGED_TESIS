using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Statistic
{
    public class DocumentsByMonthAndAreaResponse
    {
        [BsonElement("documentType")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("quantity")]
        public int Quantity { get; set; }
    }
}
