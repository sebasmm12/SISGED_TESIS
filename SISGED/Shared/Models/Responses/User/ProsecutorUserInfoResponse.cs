using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.User
{
    public class ProsecutorUserInfoResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("name")]
        public string Name { get; set; } = default!;
        [BsonElement("lastName")]
        public string LastName { get; set; } = default!;

    }
}
