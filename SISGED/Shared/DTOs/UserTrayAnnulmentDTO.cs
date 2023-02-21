namespace SISGED.Shared.DTOs
{
    public class UserTrayAnnulmentDTO
    {
        public UserTrayAnnulmentDTO(string dossierId, string currentDocumentId, string newDocumentId, string currentUserId, string newUserId)
        {
            DossierId = dossierId;
            CurrentDocumentId = currentDocumentId;
            NewDocumentId = newDocumentId;
            CurrentUserId = currentUserId;
            NewUserId = newUserId;
        }

        public UserTrayAnnulmentDTO(string dossierId, string currentDocumentId, 
                string newDocumentId, string currentUserId) : this(dossierId, currentUserId, newDocumentId, currentDocumentId, currentDocumentId)
        {
            
        }

        public string DossierId { get; set; } = default!;
        public string CurrentDocumentId { get; set; } = default!;
        public string NewDocumentId { get; set; } = default!;
        public string CurrentUserId { get; set; } = default!;
        public string NewUserId { get; set; } = default!;

        
    }
}
