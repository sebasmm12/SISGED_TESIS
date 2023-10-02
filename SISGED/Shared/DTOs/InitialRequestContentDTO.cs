using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class InitialRequestContentDTO
    {
        public string Description { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string RequestTypeId { get; set; } = default!;

        public bool HasSolicitor { get; set; }

        public string SolicitorId { get; set; } = default!;
    }
}
