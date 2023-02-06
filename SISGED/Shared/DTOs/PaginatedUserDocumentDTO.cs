namespace SISGED.Shared.DTOs
{
    public class PaginatedUserDocumentDTO
    {

        public PaginatedUserDocumentDTO() { }

        public PaginatedUserDocumentDTO(IEnumerable<UserDocumentDTO> userDocuments, int total)
        {
            UserDocuments = userDocuments;
            Total = total;
        }

        public IEnumerable<UserDocumentDTO> UserDocuments { get; set; } = default!;
        public int Total { get; set; }
    }
}
