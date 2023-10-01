using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Tray
{
    public class InputTrayResponse
    {
        public string Id { get; set; } = default!;
        public DocumentTray InputTray { get; set; } = default!;
        public Entities.DossierDocument Document { get; set; } = default!;
        public string DossierType { get; set; } = default!;
        public Client Client { get; set; } = default!;
    }
}
