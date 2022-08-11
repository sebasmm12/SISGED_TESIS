using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Entities
{
    public class Process
    {
        public Int32 Index { get; set; }
        public string Area { get; set; } = default!;
        public DateTime ReceiptDate { get; set; }
        public DateTime IssuanceDate { get; set; }
        public string IssueId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
    }
}
