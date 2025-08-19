using Microsoft.AspNetCore.Components;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Client.Components.Home;

public partial class DossiersSnapshot
{
    [Inject]
    private IHttpRepository HttpRepository { get; set; } = default!;

    [Inject]
    public ISwalFireRepository SwalFireRepository { get; set; } = default!;


    private bool isRendered = false;
    private IEnumerable<DossierSnapshotResponse>? dossiersSnapshot = default!;

    protected override async Task OnInitializedAsync()
    {
        dossiersSnapshot = await GetByCurrentMonthAsync();
        isRendered = true;
    }

    private async Task<IEnumerable<DossierSnapshotResponse>> GetByCurrentMonthAsync()
    {
        try
        {
            var dossierSnapshotResponse = await HttpRepository.GetAsync<IEnumerable<DossierSnapshotResponse>>("api/dashboards/dossiers-snapshot");
            if (dossierSnapshotResponse.Error)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el resumen de expedientes");
            }
            return dossierSnapshotResponse.Response!;
        }
        catch (Exception)
        {
            await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el resumen de expedientes");
            return Enumerable.Empty<DossierSnapshotResponse>();
        }
    }

    private int GetTotal(string dossierType)
    {
        var total = dossiersSnapshot!.FirstOrDefault(d => d.type == dossierType)?.count ?? 0;

        return total;
    }
}