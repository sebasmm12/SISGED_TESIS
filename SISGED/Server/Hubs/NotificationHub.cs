using Microsoft.AspNetCore.SignalR;
using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Server.Hubs
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationHub : Hub
    {
        public async Task SendNotificationAsync(NotificationInfoResponse notificationInfo)
        {
            await Clients.All.SendAsync("RecieveNotification", notificationInfo);
        }
    }
}
