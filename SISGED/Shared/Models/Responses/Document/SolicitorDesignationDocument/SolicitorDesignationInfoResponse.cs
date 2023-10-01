using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.SolicitorDesignationDocument
{
    public class SolicitorDesignationInfoResponse: DocumentInfoResponse
    {
        [BsonElement("content")]
        public SolicitorDesignationContentInfo Content { get; set; } = default!;
    }

    public class SolicitorDesignationContentInfo
    {
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("realizationDate")]
        public DateTime RealizationDate { get; set; } = default!;
        [BsonElement("solicitorAddress")]
        public string SolicitorAddress { get; set; } = default!;
        [BsonElement("userId")]
        public string UserId { get; set; } = default!;
        [BsonElement("solicitor")]
        public Entities.Solicitor Solicitor { get; set; } = default!;
    }
}
