using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.Models.Responses.Solicitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.DTOs
{
    public class DisciplinaryOpennessRegisterDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public string ProsecutorId { get; set; } = default!;
        public string Complainant { get; set; } = default!;
        public DateTime AudienceStartDate { get; set; } = default!;
        public DateTime AudienceEndDate { get; set; } = default!;
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public string AudienceLocation { get; set; } = default!;
        public List<Deed> ChargedDeeds { get; set; } = new List<Deed>();
    }
}
