using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Dashboards
{
    public class UserDocumentHistoryStateResponse
    {
        public string Date { get; set; } = default!;
        public IDictionary<string, int> Documents { get; set; } = default!;
    }
}