using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class ResolutionResponse : Entities.Document
    {
        public Entities.Evaluation Evaluation { get; set; } = default!;
        public ResolutionResponseContent Content { get; set; } = default!;
    }

    public class ResolutionResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime AudienceStartDate { get; set; } = default!;
        public DateTime AudienceEndDate { get; set; } = default!;
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public string Penalty { get; set; } = default!;
        public string Data { get; set; } = default!;
        public List<string> UrlAnnex { get; set; } = default!;
    }
}
