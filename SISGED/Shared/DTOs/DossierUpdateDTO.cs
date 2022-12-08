using SISGED.Shared.Models.Requests.Documents;

namespace SISGED.Shared.DTOs
{
    public class DossierUpdateDTO
    {
        public DossierUpdateDTO(DossierWrapper dossierWrapper, string dossierType, string documentId, string dossierState)
        {
            DossierWrapper = dossierWrapper;
            DossierType = dossierType;
            DocumentId = documentId;
            DossierState = dossierState;
        }

        public DossierWrapper DossierWrapper { get; set; } = default!;
        public string DossierType { get; set; } = default!;
        public string DocumentId { get; set; } = default!;
        public string DossierState { get; set; } = default!; 

        
    }
}
