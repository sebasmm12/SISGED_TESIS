using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Server.Hubs
{
    public interface INotificationHub
    {
        Task RecieveNotificationAsync(NotificationInfoResponse notification);
    }
}
