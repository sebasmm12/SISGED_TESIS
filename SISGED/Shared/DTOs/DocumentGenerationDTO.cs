namespace SISGED.Shared.DTOs
{
    public class DocumentGenerationDTO
    {
        public string PreviousDocumentId { get; set; } = default!;
        public string DocumentId { get; set; } = default!;
        public string DossierId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Sign { get; set; } = default!;
        public string GeneratedURL { get; set; } = default!;
        public string RoleId { get; set; } = default!;
    }
}
