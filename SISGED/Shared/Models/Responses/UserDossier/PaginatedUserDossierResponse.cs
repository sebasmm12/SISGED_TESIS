using SISGED.Shared.Models.Responses.Dossier;

namespace SISGED.Shared.Models.Responses.UserDossier
{
    public class PaginatedUserDossierResponse
    {
        public PaginatedUserDossierResponse(IEnumerable<DossierListResponse> userDossiers, int total)
        {
            UserDossiers = userDossiers;
            Total = total;
        }

        public PaginatedUserDossierResponse() { }

        public IEnumerable<DossierListResponse> UserDossiers { get; set; } = default!;
        public int Total { get; set; }
    }
}
