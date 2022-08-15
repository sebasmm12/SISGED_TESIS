﻿using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Responses.PublicDeed
{
    public class PublicDeedFilterResponse
    {
        public string Id { get; set; } = default!;
        public string OfficeDirection { get; set; } = default!;
        public string NotaryId { get; set; } = default!;
        public List<LegalAct> LegalActs { get; set; } = default!;
        public DateTime PublicDeedDate { get; set; }
        public string Url { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Notary { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}
