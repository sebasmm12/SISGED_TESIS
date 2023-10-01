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
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("lastName")]
        public string LastName { get; set; } = default!;
        [BsonElement("bornDate")]
        public DateTime BornDate { get; set; }
        [BsonElement("documentType")]
        public string DocumentType { get; set; } = default!;
        [BsonElement("documentNumber")]
        public string DocumentNumber { get; set; } = default!;
        [BsonElement("address")]
        public string Address { get; set; } = default!;
        [BsonElement("email")]
        public string Email { get; set; } = default!;
        [BsonElement("profile")]
        public string? Profile { get; set; }
    }
}
