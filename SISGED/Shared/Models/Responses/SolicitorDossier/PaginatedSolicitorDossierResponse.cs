namespace SISGED.Shared.Models.Responses.SolicitorDossier
{
    public class PaginatedSolicitorDossierResponse
    {
        public PaginatedSolicitorDossierResponse(IEnumerable<SolicitorDossierResponse> solicitorDossiers, int total)
        {
            SolicitorDossiers = solicitorDossiers;
            Total = total;
        }

        public IEnumerable<SolicitorDossierResponse> SolicitorDossiers { get; set; } = default!;
        public int Total { get; set; }
    }
}
