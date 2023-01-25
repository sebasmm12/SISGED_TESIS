using Microsoft.AspNetCore.Components;
using SISGED.Shared.Models.Responses.Tray;

namespace SISGED.Client.Components.Trays
{
    public partial class UserTray
    {
        [Parameter]
        public UserTrayResponse UserTrayResponse { get; set; } = default!;
    }
}