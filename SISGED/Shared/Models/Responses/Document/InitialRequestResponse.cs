using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class InitialRequestResponse : Entities.Document
    {
        public string ClientName { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public InitialRequestResponseContent Content { get; set; } = new InitialRequestResponseContent();
    }

    public class InitialRequestResponseContent
    {
        public Entities.User idcliente { get; set; } = new Entities.User();
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<string> URLAnnex { get; set; } = new List<string>();
        public DateTime CreationDate { get; set; } = default!;
    }
}
