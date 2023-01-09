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
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("apellido")]
        public string LastName { get; set; } = default!;
        [BsonElement("fechanacimiento")]
        public DateTime BornDate { get; set; }
        [BsonElement("dni")]
        public string DNI { get; set; } = default!;
        [BsonElement("direccion")]
        public string Address { get; set; } = default!;
        [BsonElement("email")]
        public string Email { get; set; } = default!;
        [BsonElement("colegiatura")]
        public string Tuition { get; set; } = default!;
        [BsonElement("oficionotarial")]
        public SolicitorOffice SolicitorOffice { get; set; } = new();
        [BsonElement("imagen")]
        public string Profile { get; set; } = default!;
        [BsonElement("exnotario")]
        public bool ExSolicitor { get; set; }
    }
}
