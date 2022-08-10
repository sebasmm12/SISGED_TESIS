using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Contracts
{
    public interface IRoleService: IGenericService
    {
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<Role> GetRoleByIdAsync(string roleId);
    }
}
