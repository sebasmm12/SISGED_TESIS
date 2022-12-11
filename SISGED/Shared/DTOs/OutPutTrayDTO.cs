namespace SISGED.Shared.DTOs
{
    public class OutPutTrayDTO
    {
        public OutPutTrayDTO(string dossierId, string currentDocumentId, string newDocumentId, string userId)
        {
            DossierId = dossierId;
            CurrentDocumentId = currentDocumentId;
            NewDocumentId = newDocumentId;
            UserId = userId;
        }

        public string DossierId { get; set; } = default!;
        public string CurrentDocumentId { get; set; } = default!;
        public string NewDocumentId { get; set; } = default!;
        public string UserId { get; set; } = default!;
    }
}
