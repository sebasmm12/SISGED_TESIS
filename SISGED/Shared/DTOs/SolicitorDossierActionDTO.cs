namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierActionDTO
    {
        public SolicitorDossierActionDTO(string solicitorDossierId, bool selected)
        {
            SolicitorDossierId = solicitorDossierId;
            Selected = selected;
        }

        public string SolicitorDossierId { get; set; } = default!;
        public bool Selected { get; set; } = default!;
    }
}
