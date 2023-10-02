using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class ResolutionContentDTO
    {
        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public DateTime AudienceStartDate { get; set; }

        public DateTime AudienceEndDate { get; set; }

        public List<string> Participants { get; set; } = new();

        public string Sanction { get; set; } = default!;

        public string ClientId { get; set; } = default!;

        public string SolicitorId { get; set; } = default!;
    }
}
