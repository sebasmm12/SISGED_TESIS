using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.DossierTray;

namespace SISGED.Client.Components.Documents;

public partial class DocumentUpdate
{
    [Inject]
    private IDocumentRepository DocumentRepository { get; set; } = default!;
    [Inject]
    private DocumentStrategy DocumentStrategy { get; set; } = default!;
    // Parameters
    [CascadingParameter(Name = "SessionAccount")]
    public SessionAccountResponse SessionAccount { get; set; } = default!;
    [CascadingParameter(Name = "WorkEnvironment")]
    public WorkEnvironment WorkEnvironment { get; set; } = default!;

    private RenderFragment? childContent;
    private DossierTrayResponse dossierTray = default!;

    protected override async Task OnInitializedAsync()
    {
        dossierTray = GetDossierTray();

        childContent = DocumentStrategy
            .GetDocumentToUpdate(dossierTray.Document!.Type)
            .RenderFragment;

        await Task.CompletedTask;
    }

    private DossierTrayResponse GetDossierTray()
    {
        var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

        var dossierTray = userTray.Value as DossierTrayResponse;

        return dossierTray!;
    }
}