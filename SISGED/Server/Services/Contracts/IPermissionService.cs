using SISGED.Shared.Entities;

namespace SISGED.Server.Services.Contracts
{
    public interface IPermissionService : IGenericService
    {
        Task<IEnumerable<Permission>> GetPermissionsAsync();
        Task<Permission> GetPermissionByIdAsync(string permissionId);
    }
}
