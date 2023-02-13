using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Models.Responses.Solicitor
{
    public class AutocompletedSolicitorResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("lastName")]
        public string LastName { get; set; } = default!;
        [BsonElement("solicitorOfficeName")]
        public string SolicitorOfficeName { get; set; } = default!;
        [BsonElement("email")]
        public string Email { get; set; } = default!;
        [BsonElement("address")]
        public string Address { get; set; } = default!;
    }
}
