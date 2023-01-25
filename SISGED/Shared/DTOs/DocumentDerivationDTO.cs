using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Shared.DTOs
{
    public class DocumentDerivationDTO
    {
        public Role? ReceiverUserRole { get; set; } = default!;
        public UserTrayResponse UserTray { get; set; } = default!;
    }
}
