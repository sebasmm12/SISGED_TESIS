namespace SISGED.Server.Services.Contracts
{
    public interface IUserConnectionManagerService
    {
        void AddUserConnection(string userId, string connectionId);
        void RemoveUserConnection(string connectionId);
        List<string> GetUserConnections(string userId);
    }
}
