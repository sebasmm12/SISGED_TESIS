namespace SISGED.Shared.DTOs
{
    public class UserDossierFilterDTO
    {
        public string? Code { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClientName { get; set; }
        public string? Type { get; set; }
        public string? State { get; set; }
    }
}
