namespace SISGED.Shared.DTOs
{
    public class ResolutionRegisterDTO : ResolutionDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime? AudienceStartDate { get; set; } = default!;
        public DateTime? AudienceEndDate { get; set; } = default!;
        public List<TextFieldDTO> Participants { get; set; } =  new() { new(string.Empty, 0) };
    }
}
