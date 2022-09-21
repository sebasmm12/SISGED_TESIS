using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.SignConclusion
{
    public class SignConclusionInfoResponse : DocumentInfoResponse
    {
        [BsonElement("content")]
        public SignConclusionContentInfo Content { get; set; } = default!;
    }

    public class SignConclusionContentInfo
    {
        [BsonElement("totalSheets")]
        public int TotalSheets { get; set; }
        [BsonElement("price")]
        public double Price { get; set; }
        [BsonElement("publicDeed")]
        public Entities.PublicDeed PublicDeed { get; set; } = default!;
        [BsonElement("solicitor")]
        public Entities.Solicitor Solicitor { get; set; } = default!;
        [BsonElement("client")]
        public Entities.User Client { get; set; } = default!;
    }
}
