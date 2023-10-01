namespace SISGED.Shared.Models.Queries.Dossier
{
    public class UserDossierPaginationQuery : PaginationQuery
    {
        public string? Code { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClientName { get; set; }
        public string? Type { get; set; }
        public string? State { get; set; }
    }
}
