using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Notification;

namespace SISGED.Server.Services.Contracts
{
    public interface INotificationService : IGenericService
    {
        Task<Notification> GetNotificationAsync(string notificationId);
        Task<IEnumerable<NotificationInfoResponse>> GetNotificationsByUserIdAsync(string userId);
        Task<Notification> RegisterNotificationAsync(Notification notification);
        Task<Notification> UpdateNotificationAsync(Notification notification);
    }
}
