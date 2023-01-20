using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.SolicitorDossier;

namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierShipmentDTO
    {
        public SolicitorDossierShipmentDTO(AutocompletedSolicitorResponse solicitor, IEnumerable<SolicitorDossierByIdsResponse> solicitorDossiers)
        {
            Solicitor = solicitor;
            SolicitorDossiers = solicitorDossiers;
        }

        public AutocompletedSolicitorResponse Solicitor { get; set; } = default!;
        public IEnumerable<SolicitorDossierByIdsResponse> SolicitorDossiers { get; set; } = default!;
    }
}
