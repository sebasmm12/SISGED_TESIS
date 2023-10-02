using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierRequestContentDTO
    {
        public string Title { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string SolicitorId { get; set; } = default!;

        public DateTime IssueDate { get; set; }

        public string ClientId { get; set; } = default!;
    }
}
