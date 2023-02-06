namespace SISGED.Shared.Models.Responses.UserDocument
{
    public class PaginatedUserDocumentResponse
    {
        public PaginatedUserDocumentResponse(IEnumerable<UserDocumentResponse> userDocuments, int total)
        {
            UserDocuments = userDocuments;
            Total = total;
        }

        public PaginatedUserDocumentResponse() { }

        public IEnumerable<UserDocumentResponse> UserDocuments { get; set; } = default!;
        public int Total { get; set; }
    }
}
