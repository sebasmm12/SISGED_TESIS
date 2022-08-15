using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class PublicDeed
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("direccionoficio")]
        public string JudicialOfficeDirection { get; set; } = default!;
        [BsonElement("titulo")]
        public string Title { get; set; } = default!;
        [BsonElement("idnotario")]
        public string NotaryId { get; set; } = default!;
        [BsonElement("actosjuridicos")]
        public List<ActoJuridico> LegalActs { get; set; } = default!;
        [BsonElement("fechaescriturapublica")]
        public DateTime PublicDeedDate { get; set; }
        [BsonElement("url")]
        public string Url { get; set; } = default!;
        [BsonElement("estado")]
        public string State { get; set; } = default!;
    }
}
