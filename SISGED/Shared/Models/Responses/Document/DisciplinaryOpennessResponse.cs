﻿using SISGED.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DisciplinaryOpennessResponse : Entities.Document
    {
        public string State { get; set; } = default!;
        public DisciplinaryOpennessResponseContent Content { get; set; } = new DisciplinaryOpennessResponseContent();
        public List<MediaRegisterDTO> URLAnnex { get; set; } = default!;

        public DisciplinaryOpennessResponse(DisciplinaryOpennessResponseContent content, List<MediaRegisterDTO> urlAnnexes)
        {
            Content = content;
            URLAnnex = urlAnnexes;
        }

        public DisciplinaryOpennessResponse() { }
    }

    public class DisciplinaryOpennessResponseContent
    {
        public string SolicitorId { get; set; } = default!;
        public string ProsecutorId { get; set; } = default!;
        public string Complainant { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime AudienceStartDate { get; set; } = default!;
        public DateTime AudienceEndDate { get; set; } = default!;
        public List<Participant> Participants { get; set; } = new List<Participant>();
        public string AudienceLocation { get; set; } = default!;
        public List<Deed> ChargedDeeds { get; set; } = new List<Deed>();
        public string URL { get; set; } = default!;
    }

    public class Participant
    {
        public string Name { get; set; } = default!;
        public Int32 Index { get; set; } = 0;
    }

    public class Deed
    {
        public string Description { get; set; } = default!;
        public Int32 Index { get; set; } = 0;
    }

}
