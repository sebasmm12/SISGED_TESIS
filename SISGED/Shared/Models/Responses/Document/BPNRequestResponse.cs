using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class BPNRequestResponse : Entities.Document
    {
        public string ClientName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string State { get; set; }
        public BPNRequestResponseContent Content { get; set; } = new BPNRequestResponseContent();
    }
    public class BPNRequestResponseContent
    {
        public Entities.User ClientId { get; set; } = new Entities.User();
        public string SolicitorAddress { get; set; }
        public Entities.Solicitor SolicitorId { get; set; }
        public string LegalAct { get; set; }
        public string ProtocolType { get; set; }
        public List<string> Grantors { get; set; } = new List<String>();
        public DateTime RealizationDate { get; set; }
        public List<GrantorList> GrantorLists { get; set; } = new List<GrantorList>();
        public List<string> URLAnnex { get; set; } = new List<string>();

    }

    public class GrantorList
    {
        public string Name { get; set; }
        //public string apellido { get; set; }
        //public int dni { get; set; }
        public Int32 Index { get; set; } = 0;
    }
}
