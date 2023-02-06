using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DocumentResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
        [BsonElement("historialcontenido")]
        public List<ContentVersion> ContentsHistory { get; set; } = default!;
        [BsonElement("historialproceso")]
        public List<Process> ProcessesHistory { get; set; } = default!;
        [BsonElement("contenido")]
        public object Content { get; set; } = default!;
        [BsonElement("estado")]
        public string State { get; set; } = default!;
        [BsonElement("evaluacion")]
        public object? Evaluation { get; set; } = default!;
        [BsonElement("fechacreacion")]
        public DateTime CreationDate { get; set; } = default!;
        [BsonElement("urlanexo")]
        public List<string> UrlAnnex { get; set; } =  default!;
    }
}
