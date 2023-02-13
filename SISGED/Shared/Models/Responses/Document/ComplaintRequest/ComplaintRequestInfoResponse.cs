using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.ComplaintRequest
{
    public class ComplaintRequestInfoResponse : DocumentInfoResponse
    {
        [BsonElement("content")]
        public ComplaintRequestContentInfo Content { get; set; } = default!;
    }

    public class ComplaintRequestContentInfo : ComplaintRequestInfo
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("complaintType")]
        public string ComplaintType { get; set; } = default!;
        [BsonElement("deliveryDate")]
        public DateTime DeliveryDate { get; set; }
    }
}
