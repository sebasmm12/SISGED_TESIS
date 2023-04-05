using Microsoft.AspNetCore.SignalR;
using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Server.Hubs
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotificationHub : Hub<INotificationHub>
    {
        public override Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext()!.Request.Query["username"];
            // username = xxxx
            return base.OnConnectedAsync();
        }
    }
}
