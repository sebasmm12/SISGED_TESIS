namespace SISGED.Server.Hubs
{
    public interface INotificationHub
    {
        Task RecieveNotificationAsync(string userId);
    }
}
