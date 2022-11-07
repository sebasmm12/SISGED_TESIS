namespace SISGED.Shared.Models.Responses.User
{
    public class PaginatedUserInfoResponse
    {
        public PaginatedUserInfoResponse(IEnumerable<UserInfoResponse> users, long totalUsers)
        {
            Users = users;
            TotalUsers = totalUsers;
        }

        public IEnumerable<UserInfoResponse> Users { get; set; } = default!;
        public long TotalUsers { get; set; }
    }
}
