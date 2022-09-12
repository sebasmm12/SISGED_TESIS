namespace SISGED.Shared.Models.Queries.Dossier
{
    public class DossierHistoryQuery
    {
        public string? State { get; set; }
        public string? ClientName { get; set; }
        public string? Type { get; set; }
        public int Page { get; set; } = 1;
        public int QuantityPerPage { get; set; } = 5;
    }
}
