using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class UserDocumentDTO
    {
        public Client Client { get; set; } = default!;
        public string DossierType { get; set; } = default!;
        public DocumentContentDTO Content { get; set; } = default!;
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public List<ContentVersion> ContentsHistory { get; set; } = default!;
        public List<Process> ProcessesHistory { get; set; } = default!;
        public string State { get; set; } = default!;
        public object? Evaluation { get; set; } = default!;
        public DateTime CreationDate { get; set; } = default!;
        public List<string> UrlAnnex { get; set; } = default!;
    }
}
