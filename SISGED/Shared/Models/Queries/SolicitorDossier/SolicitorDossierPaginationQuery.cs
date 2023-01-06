namespace SISGED.Shared.Models.Queries.SolicitorDossier
{
    public class SolicitorDossierPaginationQuery : PaginationQuery
    {
        public string SolicitorId { get; set; } = default!;
        public IEnumerable<int> Years { get; set; } = new List<int>();
    }
}
