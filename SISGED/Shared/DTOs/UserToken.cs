namespace SISGED.Shared.DTOs
{
    public class UserToken
    {
        public UserToken(string token, DateTime expiration)
        {
            Token = token;
            Expiration = expiration;
        }

        public UserToken()
        {
        }

        public string Token { get; set; } = default!;
        public DateTime Expiration { get; set; }
    }
}
