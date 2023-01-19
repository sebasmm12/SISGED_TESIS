namespace SISGED.Shared.DTOs
{
    public class ComplaintRequestRegisterDTO : ComplaintRequestDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
