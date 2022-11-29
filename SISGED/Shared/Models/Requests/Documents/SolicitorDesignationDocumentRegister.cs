﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISGED.Shared.Models.Requests.Documents
{
    public class SolicitorDesignationDocumentRegister
    {
        public string? Id { get; set; }
        public object Document { get; set; } = default!;
        public string? CurrentUserId { get; set; }
        public string? InputDocument { get; set; }

        public SolicitorDesignationDocumentRegister(object document)
        {
            Document = document;
        }

        public SolicitorDesignationDocumentRegister() {  }
    }
}
