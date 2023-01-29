namespace SISGED.Shared.Models.Requests.Documents
{
    public class DocumentEvaluationRequest
    {
        public DocumentEvaluationRequest(bool isApproved, string? comment, string documentId)
        {
            IsApproved = isApproved;
            Comment = comment;
            DocumentId = documentId;
        }

        public DocumentEvaluationRequest() {  }

        public bool IsApproved { get; set; }
        public string? Comment { get; set; }
        public string DocumentId { get; set; } = default!;
    }
}
