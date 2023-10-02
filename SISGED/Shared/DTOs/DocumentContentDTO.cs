using System.Text.Json.Serialization;

namespace SISGED.Shared.DTOs
{
    public class DocumentContentDTO
    {
        public string Title { get; set; } = default!;

        public string Code { get; set; } = default!;
    }
}
