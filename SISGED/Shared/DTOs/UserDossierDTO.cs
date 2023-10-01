using SISGED.Shared.Entities;

namespace SISGED.Shared.DTOs
{
    public class UserDossierDTO
    {
        public string Id { get; set; } = default!;
        public string Type { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public List<UserDossierDerivationDTO> Derivations { get; set; } = default!;
        public List<UserDocumentDTO> Documents { get; set; } = default!;
        public string State { get; set; } = default!;
        public DateTime StartDate { get; set; } = default!;
        public DateTime? EndDate { get; set; } = default!;
    }
}
