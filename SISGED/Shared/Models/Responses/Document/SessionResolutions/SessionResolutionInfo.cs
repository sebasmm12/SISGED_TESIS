using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.Models.Responses.Document.SessionResolutions
{
    public class SessionResolutionInfo
    {
        public SessionResolutionInfo(Client client, AutocompletedSolicitorResponse solicitor)
        {
            Client = client;
            Solicitor = solicitor;
        }

        public SessionResolutionInfo() { }

        [BsonElement("client")]
        public Client Client { get; set; } = default!;

        [BsonElement("solicitor")]
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;

        [BsonElement("previousDocument")]
        public DocumentContentDTO PreviousDocument { get; set; } = default!;
    }
}
