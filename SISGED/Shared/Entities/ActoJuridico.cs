using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class ActoJuridico
    {
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public List<Contrato> contratos { get; set; } = new List<Contrato>();
        public List<Otorgante> otorgantes { get; set; } = new List<Otorgante>();
    }
}
