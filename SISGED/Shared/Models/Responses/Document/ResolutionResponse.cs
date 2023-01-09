using MongoDB.Bson.Serialization.Attributes;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dossier;
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
        public List<MediaRegisterDTO> URLAnnex { get; set; } = default!;

        public ResolutionResponse(ResolutionResponseContent content, List<MediaRegisterDTO> urlAnnexes)
        {
            Content = content;
            URLAnnex = urlAnnexes;
        }

        public ResolutionResponse() { }
    }

    public class ResolutionResponseContent
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime AudienceStartDate { get; set; } = default!;
        public DateTime AudienceEndDate { get; set; } = default!;
        public List<string> Participants { get; set; } = default!;
        public string Penalty { get; set; } = default!;
        public string Data { get; set; } = default!;
        public string SolicitorId { get; set; } = default!;
        public string ClientId { get; set; } = default!;
    }
}
