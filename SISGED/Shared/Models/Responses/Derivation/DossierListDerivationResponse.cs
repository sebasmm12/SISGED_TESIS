using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Derivation
{
    public class DossierListDerivationResponse
    {
        [BsonElement("areaprocedencia")]
        public string OriginArea { get; set; } = default!;
        [BsonElement("areadestino")]
        public string TargetArea { get; set; } = default!;
        [BsonElement("usuarioemisor")]
        public string SenderUser { get; set; } = default!;
        [BsonElement("imagenemisor")]
        public string SenderImage{ get; set; } = default!;
        [BsonElement("usuarioreceptor")]
        public string ReceiverUser { get; set; } = default!;
        [BsonElement("imagenreceptor")]
        public string ReceiverImage { get; set; } = default!;
        [BsonElement("fechaderivacion")]
        public DateTime DerivationDate { get; set; }
        [BsonElement("estado")]
        public string State { get; set; } = default!;
        [BsonElement("tipo")]
        public string Type { get; set; } = default!;
    }
}
