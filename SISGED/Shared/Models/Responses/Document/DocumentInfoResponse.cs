

using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Document
{
    public class DocumentInfoResponse
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public List<ContentVersion> ContentHistory { get; set; } = default!;
        public List<Process> ProcessHistory { get; set; } = default!;
        public string State { get; set; } = default!;
        public List<string> AttachedUrls { get; set; } = new();
    }
}
