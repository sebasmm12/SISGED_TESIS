using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.DocumentProcess
{
    public class ProcessUser
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string UserId { get; set; } = default!;
        [BsonElement("firstName")]
        public string FirstName { get; set; } = default!;
        [BsonElement("lastName")]
        public string LastName { get; set; } = default!;
        [BsonElement("image")]
        public string Image { get; set; } = default!;
    }
}
