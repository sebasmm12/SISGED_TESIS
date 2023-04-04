using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class NotificationUsersDTO
    {
        public NotificationUsersDTO(User senderUser, User receiverUser)
        {
            SenderUser = senderUser;
            ReceiverUser = receiverUser;
        }

        public User SenderUser { get; set; } = default!;
        public User ReceiverUser { get; set; } = default!;
    }
}
