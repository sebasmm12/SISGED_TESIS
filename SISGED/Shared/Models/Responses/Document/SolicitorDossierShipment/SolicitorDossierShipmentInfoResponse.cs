using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.SolicitorDossierShipment
{
    public class SolicitorDossierShipmentInfoResponse: DocumentInfoResponse
    {
        [BsonElement("content")]
        public SolicitorDossierShipmentContentInfo Content { get; set; } = default!;
    }

    public class SolicitorDossierShipmentContentInfo
    {
        [BsonElement("title")]
        public string Title { get; set; } = default!;
        [BsonElement("description")]
        public string Description { get; set; } = default!;
        [BsonElement("solicitor")]
        public Entities.Solicitor Solicitor { get; set; } = default!;
    }
}
