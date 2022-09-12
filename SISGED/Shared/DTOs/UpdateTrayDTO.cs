namespace SISGED.Shared.DTOs
{
    public class UpdateTrayDTO
    {
        public UpdateTrayDTO(string dossierId, string recieverUserId, string senderUserId, string documentId)
        {
            DossierId = dossierId;
            RecieverUserId = recieverUserId;
            SenderUserId = senderUserId;
            DocumentId = documentId;
        }

        public string DossierId { get; set; } = default!;
        public string RecieverUserId { get; set; } = default!;
        public string SenderUserId { get; set; } = default!;
        public string DocumentId { get; set; } = default!;

        
    }
}
