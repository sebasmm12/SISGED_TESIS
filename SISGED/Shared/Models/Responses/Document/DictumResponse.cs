using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DictumResponse : Entities.Document
    {
        public string State { get; set; } = default!;
        public DictumResponseContent Content { get; set; } = new DictumResponseContent();
    }

    public class DictumResponseContent
    {
        public string Description { get; set; } = default!;
        public string ComplainantName { get; set; } = default!;
        public string Title { get; set; }
        public List<Observations> Observations { get; set; } = new List<Observations>();
        public string Conclusion { get; set; } = default!;
        public List<Recomendations> Recomendations { get; set; } = new List<Recomendations>();
        public List<string> URLAnnex { get; set; } = new List<string>(); 
    }
    public class Observations
    {
        public string Description { get; set; } = default!;
        public Int32 Index { get; set; } = 0;
    }

    public class Recomendations
    {
        public string Description { get; set; } = default!;
        public Int32 Index { get; set; } = 0;
    }
}
