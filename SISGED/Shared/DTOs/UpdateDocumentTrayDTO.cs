using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class UpdateDocumentTrayDTO
    {
        public UpdateDocumentTrayDTO(DocumentTray documentTray, string userId, string trayType)
        {
            DocumentTray = documentTray;
            UserId = userId;
            TrayType = trayType;
        }

        public DocumentTray DocumentTray { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string TrayType { get; set; } = default!;
    }
}
