using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class DocumentoExpediente
    {
        public Int32 indice { get; set; }
        public string iddocumento { get; set; }
        public string tipo { get; set; }
        public DateTime fechacreacion { get; set; }
        public DateTime fechaexceso { get; set; }
        public DateTime? fechademora { get; set; }
    }
}
