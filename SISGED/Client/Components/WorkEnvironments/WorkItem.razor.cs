using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.PublicDeed;
using System.Text.Json;

namespace SISGED.Client.Components.WorkEnvironments;

public partial class WorkItem
{
    [Inject]
    public IDocumentRepository DocumentRepository { get; set; } = default!;

    [Inject]
    public IDialogContentRepository DialogContentRepository { get; set; } = default!;

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
        "registrado",
        "evaluado",
        "modificado",
        "generado",
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
    private string DocumentTitle = string.Empty;
    private string DocumentGeneratedUrl = string.Empty;


    private string GetDocumentTitle(DossierTrayResponse dossierTray)
    {
        var currentDocumentTitle =
            JsonSerializer.Deserialize<DocumentContentDTO>(JsonSerializer.Serialize(dossierTray.Document!.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


        DocumentTitle = currentDocumentTitle!.Title;

        return DocumentTitle;
    }

    private string GetDocumentGeneratedUrl(DossierTrayResponse dossierTray)
    {
        var currentDocumentTitle =
            JsonSerializer.Deserialize<DocumentContentDTO>(JsonSerializer.Serialize(dossierTray.Document!.Content), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        DocumentGeneratedUrl = currentDocumentTitle!.GeneratedUrl;

        return DocumentGeneratedUrl;
    }

    private Placement GetPlacement()
    {
        return Item.OriginPlace == "inputs" ? Placement.Right : Placement.Left;
    }

    private bool IsToolTipVisible()
    {
        return Item.CurrentPlace != "workplace";
    }

    private async Task ShowDocumentInfoAsync(DossierTrayResponse dossierTray)
    {
        var dialogParameters = new List<DialogParameter>() { new("DocumentId", dossierTray.Document!.Id) };

        var documentInfoType = DocumentRepository.GetDocumentInfoType(dossierTray.Document.Type);

        await DialogContentRepository.ShowDialogAsync(documentInfoType, dialogParameters, "Información del Documento");
    }
}