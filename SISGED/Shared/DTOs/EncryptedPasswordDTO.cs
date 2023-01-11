namespace SISGED.Shared.DTOs
{
    public class EncryptedPasswordDTO
    {
        public EncryptedPasswordDTO(string password, string salt)
        {
            Password = password;
            Salt = salt;
        }

        public string Password { get; set; } = default!;
        public string Salt { get; set; } = default!;
    }
}
