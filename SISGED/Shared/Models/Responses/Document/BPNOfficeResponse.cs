using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Document
{
    public class BPNOfficeResponse : Entities.Document
    {
        public Evaluation Evaluation { get; set; } = new Evaluation();
        public BPNOfficeResponseContent Content { get; set; } = new BPNOfficeResponseContent();
    }

    public class BPNOfficeResponseContent
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Entities.User ClientId { get; set; } = new Entities.User();
        public string DocumentAddress { get; set; }
        public Entities.Solicitor SolicitorId { get; set; } = new Entities.Solicitor();
        public string LegalAct { get; set; }
        public string ProtocolType { get; set; }
        public List<BPNGranter> Granters { get; set; } = new List<BPNGranter>();
        public DateTime? RealizationDate { get; set; }
        public string Data { get; set; }
        public List<string> URLannex { get; set; } = new List<string>();
    }

    public class BPNGranter
    {
        public string Name { get; set; }
        public Int32 Index { get; set; } = 0;
    }
}
