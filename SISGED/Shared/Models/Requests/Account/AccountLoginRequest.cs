namespace SISGED.Shared.Models.Requests.Account
{
    public class AccountLoginRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
