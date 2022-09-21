namespace SISGED.Shared.Models.Responses.Document.Resolution
{
    public class ResolutionInfoResponse: DocumentInfoResponse
    {
        public ResolutionContentInfo Content { get; set; } = default!;
    }

    public class ResolutionContentInfo
    {
        public string Description { get; set; } = default!;
        public string Title { get; set; } = default!;
        public DateTime AudienceStartDate { get; set; }
        public DateTime AudienceEndDate { get; set; }
        public List<string> Participants { get; set; } = new();
        public string Sanction { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}
