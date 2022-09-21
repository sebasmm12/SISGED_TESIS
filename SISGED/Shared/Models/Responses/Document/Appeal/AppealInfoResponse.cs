namespace SISGED.Shared.Models.Responses.Document.Appeal
{
    public class AppealInfoResponse
    {
        public AppealContentInfo Content { get; set; } = default!;
    }

    public class AppealContentInfo
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime AppealDate { get; set; }
        public string Url { get; set; } = default!;
    }
}
