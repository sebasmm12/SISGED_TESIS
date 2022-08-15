using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class TrayService : ITrayService
    {
       private readonly IMongoCollection<Tray> _traysCollection;
       private readonly IMongoCollection<User> _usersCollection;
        public string CollectionName => "bandejas";
        public string SecondCollectionName => "usuarios";

        public TrayService(IMongoDatabase mongoDatabase)
        {
            _traysCollection = mongoDatabase.GetCollection<Tray>(CollectionName);
            _usersCollection = mongoDatabase.GetCollection<User>(SecondCollectionName);
        }
        
        public async Task RegisterUserTrayAsync(string type, string userId)
        {
            Tray userTray = new(type, userId);

            await _traysCollection.InsertOneAsync(userTray);

            if (string.IsNullOrEmpty(userTray.Id)) throw new Exception($"No se pudo registrar la bandeja del usuario {userId} al sistema");
        }


    }
}
