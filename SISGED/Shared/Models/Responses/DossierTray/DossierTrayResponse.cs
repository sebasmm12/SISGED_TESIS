using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.DossierTray
{
    public class DossierTrayResponse
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("idexpediente")]
        public string DossierId { get; set; } = default!;
        [BsonElement("cliente")]
        public Client? Client { get; set; } 
        [BsonElement("documento")]
        public DocumentResponse? Document { get; set; } 
        [BsonElement("documentosobj")]
        public List<DocumentResponse>? DocumentObjects { get; set; }
        [BsonElement("tipo")]
        public string? Type { get; set; }
    }
}
