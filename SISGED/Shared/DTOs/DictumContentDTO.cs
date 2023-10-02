namespace SISGED.Shared.DTOs
{
    public class DictumContentDTO
    {
        public string ComplaintId { get; set; } = default!;

        public string SolicitorId { get; set; } = default!;

        public string Title { get; set; } = default!;

        public List<string> Observations { get; set; } = new();

        public string Conclusion { get; set; } = default!;

        public List<string> Recommendations { get; set; } = new();
    }
}
