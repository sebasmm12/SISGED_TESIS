using Microsoft.AspNetCore.SignalR;
using SISGED.Server.Services.Contracts;

namespace SISGED.Server.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        private readonly IUserConnectionManagerService _userConnectionManagerService;

        public NotificationHub(IUserConnectionManagerService userConnectionManagerService)
        {
            _userConnectionManagerService = userConnectionManagerService;
        }

        public async override Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()!.Request.Query["userId"];

            _userConnectionManagerService.AddUserConnection(userId!, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionId = Context.ConnectionId;

            _userConnectionManagerService.RemoveUserConnection(connectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
