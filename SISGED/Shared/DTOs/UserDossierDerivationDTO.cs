using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class UserDossierDerivationDTO
    {
        public string OriginArea { get; set; } = default!;
        public string TargetArea { get; set; } = default!;
        public string SenderUser { get; set; } = default!;
        public string SenderImage { get; set; } = default!;
        public string ReceiverUser { get; set; } = default!;
        public string ReceiverImage { get; set; } = default!;
        public DateTime DerivationDate { get; set; }
        public string State { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
