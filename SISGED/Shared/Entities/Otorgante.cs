using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class Otorgante
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
    }

    public class OtorganteDTO
    {
        public string nombre { get; set; }
        public Int32 index { get; set; } = 0;
    }
}
