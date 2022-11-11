namespace SISGED.Shared.Models.Responses.Document.UserRequest
{
    public class PaginatedUserRequest
    {
        public PaginatedUserRequest(IEnumerable<UserRequestWithPublicDeedResponse> userRequests, long totalUserRequests)
        {
            UserRequests = userRequests;
            TotalUserRequests = totalUserRequests;
        }

        public IEnumerable<UserRequestWithPublicDeedResponse> UserRequests { get; set; } = default!;
        public long TotalUserRequests { get; set; }
    }
}
