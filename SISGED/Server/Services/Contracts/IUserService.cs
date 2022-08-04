using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.User;

namespace SISGED.Server.Services.Contracts
{
    public interface IUserService : IGenericService
    {
        Task CreateUserAsync(User user);
        Task<IEnumerable<ProsecutorUserInfoResponse>> GetProsecutorUsersAsync();
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<List<User>> GetUsersAsync();
        Task<IEnumerable<User>> GetUsersByStateAsync(string userState);
        Task UpdateUserAsync(User user);
        Task UpdateUserPasswordAsync(string userId, string password);
        Task UpdateUserStateAsync(string userId, string state);
        Task<bool> VerifyUserExistsAsync(string userId);
    }
}
