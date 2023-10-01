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

        [BsonElement("type")]
        public string Type { get; set; } = default!;

        [BsonElement("userName")]
        public string UserName { get; set; } = default!;

        [BsonElement("password")]
        public string Password { get; set; } = default!;
        [BsonElement("salt")]
        public string Salt { get; set; } = default!;

        [BsonElement("data")]
        public UserData Data { get; set; } = new UserData();

        [BsonElement("state")]
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

        public string GetFullName()
        {
            return Data.Name + " " + Data.LastName;
        }
    }
}
