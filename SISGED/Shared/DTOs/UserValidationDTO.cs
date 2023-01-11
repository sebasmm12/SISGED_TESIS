namespace SISGED.Shared.DTOs
{
    public class UserValidationDTO
    {
        public UserValidationDTO(bool result, string? errorMessage)
        {
            Result = result;
            ErrorMessage = errorMessage;
        }

        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
