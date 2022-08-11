using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DocumentResponse
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public List<ContentVersion> ContentHistory { get; set; } = default!;
        public List<Process> ProcessHistory { get; set; } = default!;
        public Object Content { get; set; } = default!;
        public Object State { get; set; } = default!;
        public Object Evaluation { get; set; } = default!;
        public DateTime CreationDate { get; set; } = default!;
        public List<string> UrlAnnex { get; set; } = = default!;
    }
}
