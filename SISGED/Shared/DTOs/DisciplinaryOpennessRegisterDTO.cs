namespace SISGED.Shared.DTOs
{
    public class DisciplinaryOpennessRegisterDTO : DisciplinaryOpennessDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime? AudienceStartDate { get; set; } = default!;
        public DateTime? AudienceEndDate { get; set; } = default!;
        public List<TextFieldDTO> Participants { get; set; } = new() { new(string.Empty, 0) };
        public string AudienceLocation { get; set; } = default!;
        public List<TextFieldDTO> ChargedDeeds { get; set; } = new() { new(string.Empty, 0) };
    }
}
