namespace SISGED.Shared.Models.Requests.User
{
    public class UserPasswordUpdateRequest
    {
        public string UserId { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
