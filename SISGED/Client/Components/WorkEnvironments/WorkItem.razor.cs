using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Helpers;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.PublicDeed;

namespace SISGED.Client.Components.WorkEnvironments;

public partial class WorkItem
{
    private readonly IEnumerable<string> accepetedStatusToDocument = new List<string>
    {
        "registered",
        "evaluated"
    };

    private readonly IEnumerable<string> acceptedDocumentTypes = new List<string>
    {
        "OficioDesignacionNotario",
        "ConclusionFirma",
        "OficioBPN",
        "ResultadoBPN"
    };

    private readonly IEnumerable<string> acceptedStatusToInfo = new List<string>
    {
        "inspace",
        "registered",
        "evaluated",
        "choosen"
    };

    private readonly IEnumerable<string> acceptedStatusToPublicDeed = new List<string>
    {
        "choosen"
    };

    [Parameter] public Item Item { get; set; } = default!;

    [Parameter] public PublicDeedFilterResponse PublicDeed { get; set; } = default!;

    private string? ToolColor => Item.OriginPlace == "tools" ? "color-tool" : null;


    private string GetDocumentTitle(DossierTrayResponse dossierTray)
    {
        var currentDocumentTitle =
            JsonSerializer.Deserialize<DocumentContentDTO>(JsonSerializer.Serialize(dossierTray.Document!.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return currentDocumentTitle!.Title;
    }
}