using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Shared.Models.Responses.PublicDeed;

namespace SISGED.Client.Components.WorkEnvironments
{
    public partial class WorkItem
    {
        [Parameter]
        public Item Item { get; set; } = default!;
        [Parameter]
        public PublicDeedFilterResponse PublicDeed { get; set; } = default!;
        
        private readonly IEnumerable<string> accepetedStatusToDocument = new List<string>()
        {
            "registered",
            "evaluated"
        };

        private readonly IEnumerable<string> acceptedStatusToPublicDeed = new List<string>()
        {
            "choosen"
        };

        private readonly IEnumerable<string> acceptedStatusToInfo = new List<string>()
        {
            "inspace",
            "registered",
            "evaluated",
            "choosen"
        };

        private readonly IEnumerable<string> acceptedDocumentTypes = new List<string>()
        {
            "OficioDesignacionNotario",
            "ConclusionFirma",
            "OficioBPN",
            "ResultadoBPN"
        };

        private string? ToolColor => Item.OriginPlace == "tools" ? "color-tool": null;
    }
}