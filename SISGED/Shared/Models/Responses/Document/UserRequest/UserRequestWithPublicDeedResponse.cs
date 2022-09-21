using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Document.UserRequest
{
    public class UserRequestWithPublicDeedResponse : UserRequestDocumentResponse
    {
        [BsonElement("dossierUrl")]
        public string DossierUrl { get; set; } = default!;
    }
}
