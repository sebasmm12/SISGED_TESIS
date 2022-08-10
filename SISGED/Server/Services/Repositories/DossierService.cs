using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class DossierService : IDossierService
    {
        private readonly IMongoCollection<Dossier> _dossiersCollection;

        public DossierService(IMongoDatabase mongoDatabase)
        {
            _dossiersCollection = mongoDatabase.GetCollection<Dossier>(CollectionName);
        }

        public string CollectionName => "expedientes";
    }
}
