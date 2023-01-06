using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierShipmentRegisterDTO
    {
        public string Title { get; set; } = default!;
        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<string> SolicitorDossiers { get; set; } = new();
    }
}
