namespace SISGED.Shared.Models.Responses.Statistic
{
    public class EvaluatedDocumentsResponse
    {
        public string DocumentType { get; set; } = default!;
        public int ApprovedDocuments { get; set; }
        public int RejectedDocument { get; set; }
    }
}
