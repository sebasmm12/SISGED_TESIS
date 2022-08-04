using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class TrayService : ITrayService
    {
       private readonly IMongoCollection<Tray> _traysCollection;
        public string CollectionName => "bandejas";

        public TrayService(IMongoDatabase mongoDatabase)
        {
            _traysCollection = mongoDatabase.GetCollection<Tray>(CollectionName);
        }
        
        public async Task RegisterUserTrayAsync(string type, string userId)
        {
            Tray userTray = new(type, userId);

            await _traysCollection.InsertOneAsync(userTray);

            if (string.IsNullOrEmpty(userTray.Id)) throw new Exception($"No se pudo registrar la bandeja del usuario {userId} al sistema");
        }
    }
}
