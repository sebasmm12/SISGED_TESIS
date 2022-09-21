namespace SISGED.Shared.Models.Responses.Document.Dictum
{
    public class DictumInfoResponse : DocumentInfoResponse
    {
        public DictumContentInfo Content { get; set; } = default!;
    }

    public class DictumContentInfo
    {
        public string Description { get; set; } = default!;
        public string ComplainantName { get; set; } = default!;
        public string Title { get; set; } = default!;
        public List<string> Observations { get; set; } = new();
        public string Conclusion { get; set; } = default!;
        public List<string> Recommendations { get; set; } = new();
    }
}
