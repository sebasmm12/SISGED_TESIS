using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class Folder
    {
        public string Id { get; set; } = default!;
        public DateTime Date { get; set; }
        public List<FolderDocument> Documents { get; set; } = default!;
    }
}
