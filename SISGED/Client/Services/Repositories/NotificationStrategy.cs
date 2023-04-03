using SISGED.Client.Services.Contracts;
using System.Linq;

namespace SISGED.Client.Services.Repositories
{
    public class NotificationStrategy
    {
        private readonly Dictionary<bool, string> _notificationStates = new()
        {
            { true, "seen" },
            { false, "no-seen" },
        }; 

        public string GetNotificationStatus(bool state)
        {
            string notificationStatus = _notificationStates.FirstOrDefault(notificationState => notificationState.Key == state).Value;

            return notificationStatus;
        }
    }
}
