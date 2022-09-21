using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class SignConclusionResponse : Entities.Document
    {
        public SignConclusionResponseContent Content { get; set; } = new SignConclusionResponseContent();
    }
    public class SignConclusionResponseContent
    {
        public Entities.PublicDeed PublicDeedId { get; set; } = new Entities.PublicDeed();
        public Entities.Solicitor SolicitorId { get; set; } = new Entities.Solicitor();
        public Entities.User ClientId { get; set; } = new Entities.User();
        public Int32 PageQuantity { get; set; } = 2;
        public double Price { get; set; } = 0;
        public List<string> URLAnnex { get; set; } = new List<string>();

    }
}
