using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class ComplaintRequestResponse : Entities.Document
    {
        public string ClientName { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;

        public ComplaintRequestResponseContent Content { get; set; } = new ComplaintRequestResponseContent();
    }
    public class ComplaintRequestResponseContent
    {
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ClientName { get; set; } = default!;
        public DateTime DeliveryDate { get; set; } = default!;
        public string URLData { get; set; } = default!;
        public List<string> URLAnnex { get; set; } = new List<string>();

    }
}
