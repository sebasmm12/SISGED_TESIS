using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DisciplinaryOpennessRequestInfo
    {
        public DisciplinaryOpennessRequestInfo(Client client, AutocompletedSolicitorResponse solicitor/*, ProsecutorUserInfoResponse prosecutor*/)
        {
            Client = client;
            Solicitor = solicitor;
            //Prosecutor = prosecutor;
        }

        public DisciplinaryOpennessRequestInfo() { }

        [BsonElement("client")]
        public Client Client { get; set; } = default!;
        [BsonElement("solicitor")]
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        //[BsonElement("prosecutor")]
        //public ProsecutorUserInfoResponse Prosecutor { get; set; } = default!;
    }
}
