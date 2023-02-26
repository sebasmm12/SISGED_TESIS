using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.SolicitorDossier;

namespace SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment
{
    public class SolicitorDossierShipmentInfoResponse: DocumentInfoResponse
    {
        [BsonElement("content")]
        public SolicitorDossierShipmentContentInfo Content { get; set; } = default!;
    }

    public class SolicitorDossierShipmentContentInfo
    {
        [BsonElement("code")]
        public string Code { get; set; } = default!;
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("solicitor")]
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        [BsonElement("solicitorDossiers")]
        public IEnumerable<SolicitorDossierResponse> SolicitorDossiers { get; set; } = default!;
    }
}
