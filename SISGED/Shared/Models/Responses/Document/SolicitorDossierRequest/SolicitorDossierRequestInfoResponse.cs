using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest
{
    public class SolicitorDossierRequestInfoResponse : DocumentInfoResponse
    {
        [BsonElement("content")]
        public SolicitorDossierRequestContentInfo Content { get; set; } = default!;
    }

    public class SolicitorDossierRequestContentInfo
    {
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("solicitor")]
        public Entities.Solicitor Solicitor { get; set; } = default!;
        [BsonElement("issueDate")]
        public DateTime IssueDate { get; set; }
    }
}
