namespace SISGED.Shared.DTOs
{
    public class SolicitorDossierRequestRegisterDTO : SolicitorDossierRequestDTO
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
