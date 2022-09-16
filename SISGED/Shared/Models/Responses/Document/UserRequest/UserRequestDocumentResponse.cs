using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Generics.Document;

namespace SISGED.Shared.Models.Responses.Document.UserRequest
{
    public class UserRequestDocumentResponse : GeneralDocument
    {
        [BsonElement("content")]
        public InitialRequestContent Content { get; set; } = default!;
    }
}
