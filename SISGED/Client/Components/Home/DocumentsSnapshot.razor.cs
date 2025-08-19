using Microsoft.AspNetCore.Components;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Client.Components.Home;

public partial class DocumentsSnapshot
{
    [Inject]
    private IHttpRepository HttpRepository { get; set; } = default!;

    [Inject]
    public ISwalFireRepository SwalFireRepository { get; set; } = default!;

    [CascadingParameter(Name = "SessionAccount")]
    public SessionAccountResponse SessionAccount { get; set; } = default!;

    private bool isRendered = false;
    private UserDocumentSnapshotResponse? documentSnapshotResponse = default!;

    protected override async Task OnInitializedAsync()
    {
        documentSnapshotResponse = await GetByUserAsync(SessionAccount.User.Id);

        isRendered = true;
    }

    private async Task<UserDocumentSnapshotResponse?> GetByUserAsync(string userId)
    {
        try
        {
            var userDocumentsSnapshotResponse = await HttpRepository.GetAsync<UserDocumentSnapshotResponse>($"api/dashboards/user-documents-snapshot/{userId}");

            if (userDocumentsSnapshotResponse.Error)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el resumen de los documentos");
            }

            return userDocumentsSnapshotResponse.Response!;
        }
        catch (Exception)
        {
            await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el resumen de los documentos");
            return null;
        }
    }
}