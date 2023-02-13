using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.Models.Responses.Document
{
    public class ComplaintRequestInfo
    {
        public ComplaintRequestInfo(Client client, AutocompletedSolicitorResponse solicitor)
        {
            Client = client;
            Solicitor = solicitor;
        }

        public ComplaintRequestInfo() { }

        [BsonElement("client")]
        public Client Client { get; set; } = default!;
        [BsonElement("solicitor")]
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
    }
}
