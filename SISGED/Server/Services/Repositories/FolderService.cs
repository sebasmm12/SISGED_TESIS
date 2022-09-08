using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class FolderService : IFolderService
    {
        private readonly IMongoCollection<Folder> _foldersCollection;
        public string CollectionName => "carpetas";
        public FolderService(IMongoDatabase mongoDatabase)
        {
            _foldersCollection = mongoDatabase.GetCollection<Folder>(CollectionName);
        }

        public async Task<IEnumerable<Folder>> GetAsync()
        {
            return await _foldersCollection.Find(carpeta => true).ToListAsync();
        }
    }
}
