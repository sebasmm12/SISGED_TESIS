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

        public string DossierId { get; set; }
        public string CurrentDocumentId { get; set; }
        public string NewDocumentId { get; set; }
        public string UserId { get; set; }
    }
}
