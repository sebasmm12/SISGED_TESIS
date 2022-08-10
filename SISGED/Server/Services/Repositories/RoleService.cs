using MongoDB.Driver;
using SISGED.Server.Services.Contracts;
using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Repositories
{
    public class RoleService : IRoleService
    {
        private readonly IMongoCollection<Role> _rolesCollection;
        public string CollectionName => "roles";

        public RoleService(IMongoDatabase mongoDatabase)
        {
           _rolesCollection = mongoDatabase.GetCollection<Role>(CollectionName);
        }

        public async Task<Role> GetRoleByIdAsync(string roleId)
        {
            var role = await _rolesCollection.Find(role => role.Id == roleId).FirstOrDefaultAsync();

            if (role is null) throw new Exception($"No se pudo encontrar el rol con el identificador { roleId }");

            return role;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var roles = await _rolesCollection.Find(_ => true).ToListAsync();

            if (roles is null) throw new Exception("No se pudo encontrar los roles registrados");

            return roles;
        }
    }
}
