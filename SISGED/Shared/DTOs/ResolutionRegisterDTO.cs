using SISGED.Shared.Models.Responses.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class ResolutionRegisterDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime AudienceStartDate { get; set; } = default!;
        public DateTime AudienceEndDate { get; set; } = default!;
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public string Penalty { get; set; } = default!;
    }
}
