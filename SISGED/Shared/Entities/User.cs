using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;

        [BsonElement("tipo")]
        public string Type { get; set; } = default!;

        [BsonElement("usuario")]
        public string UserName { get; set; } = default!;

        [BsonElement("clave")]
        public string Password { get; set; } = default!;

        [BsonElement("datos")]
        public UserInfo Data { get; set; } = new UserInfo();

        [BsonElement("estado")]
        public string State { get; set; } = default!;

        [BsonElement("rol")]
        public string Rol { get; set; } = default!;

        public void SetProfile(string? profile)
        {
            Data.Profile = profile;
        }

        public string? GetProfile()
        {
            return Data.Profile;
        }
    }
}
