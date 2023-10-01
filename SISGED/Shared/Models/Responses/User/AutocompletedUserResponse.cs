using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.User
{
    public class AutocompletedUserResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("userName")]
        public string UserName { get; set; } = default!;
        [BsonElement("roleName")]
        public string RoleName { get; set; } = default!;
        [BsonElement("document")]
        public string Document { get; set; } = default!;
    }
}
