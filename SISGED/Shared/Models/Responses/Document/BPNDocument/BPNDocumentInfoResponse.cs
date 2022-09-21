using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.BPNDocument
{
    public class BPNDocumentInfoResponse : DocumentInfoResponse
    {
        [BsonElement("content")]
        public BPNDocumentContentInfo Content { get; set; } = default!;
    }

    public class BPNDocumentContentInfo
    {
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("client")]
        public Entities.User Client { get; set; } = default!;
        [BsonElement("solicitor")]
        public Entities.Solicitor Solicitor { get; set; } = default!;
        [BsonElement("juridicalAct")]
        public string JuridicalAct { get; set; } = default!;
        [BsonElement("protocolType")]
        public string ProtocolType { get; set; } = default!;
        [BsonElement("grantors")]
        public List<string> Grantors { get; set; } = new();
        [BsonElement("realizationDate")]
        public DateTime RealizationDate { get; set; }
    }
}
