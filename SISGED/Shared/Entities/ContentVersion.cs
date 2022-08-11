using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class ContentVersion
    {
        public Int32 Version { get; set; }
        public DateTime ModificationDate { get; set; } = DateTime.Now;
        public string Url { get; set; } = default!;
    }
}
