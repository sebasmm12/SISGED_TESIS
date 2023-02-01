using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;

namespace SISGED.Shared.Models.Responses.UserDocument
{
    public class UserDocumentResponse : DocumentResponse
    {
        [BsonElement("cliente")]
        public Client Client { get; set; } = default!;
        [BsonElement("tipoExpediente")]
        public string DossierType { get; set; } = default!;
        
    }
}
