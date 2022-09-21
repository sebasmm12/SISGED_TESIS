using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class AppealResponse : Entities.Document
    {
        public Entities.Evaluation Evaluation { get; set; } = new Entities.Evaluation();
        public AppealResponseContent Content { get; set; } = new AppealResponseContent();
    }


    public class AppealResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime AppealDate { get; set; } = default!;
        public string Data { get; set; } = default!;
        public List<string> URLAnnex { get; set; } = new List<string>();
    }
}