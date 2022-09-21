using SISGED.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
