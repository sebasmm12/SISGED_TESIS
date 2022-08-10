using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class UserData
    {
        [BsonElement("nombre")]
        public string Name { get; set; } = default!;
        [BsonElement("apellido")]
        public string LastName { get; set; } = default!;
        [BsonElement("fechanacimiento")]
        public DateTime BornDate { get; set; }
        [BsonElement("tipodocumento")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("numerodocumento")]
        public string DocumentNumber { get; set; } = default!;
        [BsonElement("direccion")]
        public string Address { get; set; } = default!;
        [BsonElement("email")]
        public string Email { get; set; } = default!;
        [BsonElement("imagen")]
        public string? Profile { get; set; }
    }
}
