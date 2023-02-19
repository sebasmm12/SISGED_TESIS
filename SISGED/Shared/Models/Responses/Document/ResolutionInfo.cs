using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class ResolutionInfo
    {
        public ResolutionInfo(Client client, AutocompletedSolicitorResponse solicitor)
        {
            Client = client;
            Solicitor = solicitor;
        }

        public ResolutionInfo() { }

        [BsonElement("client")]
        public Client Client { get; set; } = default!;
        [BsonElement("solicitor")]
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
    }
}
