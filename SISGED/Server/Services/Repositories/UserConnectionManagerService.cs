using SISGED.Server.Services.Contracts;

namespace SISGED.Server.Services.Repositories
{
    public class UserConnectionManagerService : IUserConnectionManagerService
    {
        private static readonly Dictionary<string, List<string>> userConnections = new();
        private static readonly object userConnectionMapLocker = new();

        public void AddUserConnection(string userId, string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                if(!userConnections.ContainsKey(userId)) userConnections[userId] = new List<string>();

                userConnections[userId].Add(connectionId);
            }
        }

        public List<string> GetUserConnections(string userId)
        {
            var userConnection = new List<string>();

            lock (userConnectionMapLocker) userConnection = userConnections[userId];

            return userConnection;
        }

        public void RemoveUserConnection(string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                var userConnection = userConnections.FirstOrDefault(userConnection => userConnection.Value.Contains(connectionId));

                if (string.IsNullOrEmpty(userConnection.Key))
                    return;

                userConnections[userConnection.Key].Remove(connectionId);
            }
        }
    }
}
