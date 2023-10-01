using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SISGED.Shared.Entities
{
    public class Solicitor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("lastName")]
        public string LastName { get; set; } = default!;
        [BsonElement("bornDate")]
        public DateTime BornDate { get; set; }
        [BsonElement("dni")]
        public string DNI { get; set; } = default!;
        [BsonElement("address")]
        public string Address { get; set; } = default!;
        [BsonElement("email")]
        public string Email { get; set; } = default!;
        [BsonElement("tuition")]
        public string Tuition { get; set; } = default!;
        [BsonElement("solicitorOffice")]
        public SolicitorOffice SolicitorOffice { get; set; } = new();
        [BsonElement("profile")]
        public string Profile { get; set; } = default!;
        [BsonElement("exSolicitor")]
        public bool ExSolicitor { get; set; }
    }
}
