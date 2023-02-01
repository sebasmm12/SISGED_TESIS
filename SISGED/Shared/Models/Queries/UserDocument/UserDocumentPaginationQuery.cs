namespace SISGED.Shared.Models.Queries.UserDocument
{
    public class UserDocumentPaginationQuery : PaginationQuery
    {
        public string? Code { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClientName { get; set; }
        public string? DossierType { get; set; }
        public string? State { get; set; }
    }
}
