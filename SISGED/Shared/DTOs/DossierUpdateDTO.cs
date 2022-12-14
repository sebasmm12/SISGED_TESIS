using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;

namespace SISGED.Shared.DTOs
{
    public class DossierUpdateDTO
    {
        public DossierUpdateDTO(DossierWrapper dossierWrapper, string dossierType, 
            string dossierState, DossierDocument dossierDocument)
        {
            DossierWrapper = dossierWrapper;
            DossierType = dossierType;
            DossierState = dossierState;
            DossierDocument = dossierDocument;
        }

        public DossierWrapper DossierWrapper { get; set; } = default!;
        public string DossierType { get; set; } = default!;

        public string DossierState { get; set; } = default!;
        public DossierDocument DossierDocument { get; set; } = default!;

    }
   
}
