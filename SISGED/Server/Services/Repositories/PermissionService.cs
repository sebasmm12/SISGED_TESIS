using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class PermissionService : IPermissionService
    {
        private readonly IMongoCollection<Permission> _permissionCollection;
        public string CollectionName => "permisos";
        public PermissionService(IMongoDatabase mongoDatabase)
        {
            _permissionCollection = mongoDatabase.GetCollection<Permission>(CollectionName);
        }

        public async Task<Permission> GetPermissionByIdAsync(string permissionId)
        {
            var permissions = await _permissionCollection.Find(permission => permission.Id == permissionId).FirstOrDefaultAsync();

            if (permissions is null) throw new Exception($"No se pudo encontrar el permiso con el identificador { permission }");

            return permissions;
        }

        public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            var permissions = await _permissionCollection.Find(_ => true).ToListAsync();

            if (permissions is null) throw new Exception("No se pudo encontrar los permisos registrados");

            return permissions;
        }
    }
}
