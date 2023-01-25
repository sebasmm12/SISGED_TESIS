using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Models.Responses.DossierTray;

namespace SISGED.Shared.Models.Responses.Tray
{
    public class InputOutputTrayResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public List<DossierTrayResponse> OutputDossier { get; set; }
          = new List<DossierTrayResponse>();
        public List<DossierTrayResponse> InputDossier { get; set; }
          = new List<DossierTrayResponse>();
    }
}
