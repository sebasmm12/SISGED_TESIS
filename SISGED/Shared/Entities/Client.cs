using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class Client
    {
        public string Name { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
    }
}
