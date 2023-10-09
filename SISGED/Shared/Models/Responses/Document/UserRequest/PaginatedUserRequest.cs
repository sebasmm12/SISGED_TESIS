namespace SISGED.Shared.Models.Responses.Document.UserRequest
{
    public class PaginatedUserRequest
    {
        public PaginatedUserRequest(IEnumerable<UserRequestResponse> userRequests, long totalUserRequests)
        {
            UserRequests = userRequests;
            TotalUserRequests = totalUserRequests;
        }

        public IEnumerable<UserRequestResponse> UserRequests { get; set; } = default!;
        public long TotalUserRequests { get; set; }
    }
}
