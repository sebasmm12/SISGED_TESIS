using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class SessionResolutionRegisterDTO
    {
        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string PreviousDocumentId { get; set; } = default!;

        public Client Client { get; set; } = default!;

        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;

        public DocumentContentDTO DocumentContent { get; set; } = default!;
    }
}
