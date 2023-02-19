using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.SolicitorDossierRequest
{
    public class SolicitorDossierRequestInfoResponse : DocumentInfoResponse
    {
        [BsonElement("content")]
        public SolicitorDossierRequestContentInfo Content { get; set; } = default!;
    }

    public class SolicitorDossierRequestContentInfo : SolicitorDossierRequestInfo
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("issueDate")]
        public DateTime IssueDate { get; set; }
    }
}
