namespace SISGED.Shared.DTOs
{
    public class DisciplinaryOpennessContentDTO
    {
        public string SolicitorId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime AudienceStartDate { get; set; } = DateTime.UtcNow.AddHours(-5);

        public DateTime AudienceEndDate { get; set; } = DateTime.UtcNow.AddDays(-5);

        public List<string> Participants { get; set; } = new();

        public string AudiencePlace { get; set; } = default!;

        public List<string> ImputedFacts { get; set; } = new();

        public string ClientId { get; set; } = default!;
    }
}
