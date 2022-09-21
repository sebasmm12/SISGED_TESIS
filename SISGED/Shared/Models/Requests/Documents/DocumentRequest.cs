using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Requests.Documents
{
    public class DocumentRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Type { get; set; }
        public List<Entities.ContentVersion> ContentHistory { get; set; } = new List<Entities.ContentVersion>();
        public List<Entities.Process> ProcessHistory { get; set; } = new List<Entities.Process>();
        public Object Content { get; set; }
        public Object State { get; set; }
        public Object Evaluation { get; set; }
        public DateTime CreationDate { get; set; }
        public List<string> URLAnnex { get; set; } = new List<string>();
    }
}
