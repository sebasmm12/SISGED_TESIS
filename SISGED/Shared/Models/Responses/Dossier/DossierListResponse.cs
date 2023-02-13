using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.UserDocument;
using SISGED.Shared.Models.Responses.Derivation;

namespace SISGED.Shared.Models.Responses.Dossier
{
    public class DossierListResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("cliente")]
        public Client Client { get; set; } = default!;
        [BsonElement("fechainicio")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddHours(-5);
        [BsonElement("fechafin")]
        public DateTime? EndDate { get; set; }
        [BsonElement("documentos")]
        public List<UserDocumentResponse> Documents { get; set; } = new();
        [BsonElement("derivaciones")]
        public List<DossierListDerivationResponse> Derivations { get; set; } = new();
        [BsonElement("estado")]
        public string State { get; set; } = default!;
    }
}
