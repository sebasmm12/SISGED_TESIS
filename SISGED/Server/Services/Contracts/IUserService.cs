using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.User;
using SISGED.Shared.Models.Responses.User;

namespace SISGED.Server.Services.Contracts
{
    public interface IUserService : IGenericService
    {
        Task<long> CountUsersAsync();
        Task CreateUserAsync(User user);
        Task<IEnumerable<AutocompletedUserResponse>> GetAutocompletedUsersAsync(string userName);
        Task<IEnumerable<ClientUserInfoResponse>> GetClientUsersAsync(string userName);
        Task<IEnumerable<ProsecutorUserInfoResponse>> GetProsecutorUsersAsync();
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<List<User>> GetUsersAsync(UserPaginationQuery userPaginationQuery);
        Task<IEnumerable<User>> GetUsersByStateAsync(string userState);
        Task UpdateUserAsync(User user);
        Task UpdateUserPasswordAsync(string userId, string password);
        Task UpdateUserStateAsync(string userId, string state);
        Task<bool> VerifyUserExistsAsync(string userId);
        Task<bool> VerifyUserLoginAsync(string username, string password);
    }
}
