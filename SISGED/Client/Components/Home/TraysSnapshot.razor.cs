using Microsoft.AspNetCore.Components;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Client.Components.Home;

public partial class TraysSnapshot
{
    [Inject]
    private IHttpRepository HttpRepository { get; set; } = default!;

    [Inject]
    public ISwalFireRepository SwalFireRepository { get; set; } = default!;

    [CascadingParameter(Name = "SessionAccount")]
    public SessionAccountResponse SessionAccount { get; set; } = default!;

    private bool isRendered = false;
    private UserTraysSnapshotResponse? userTraysSnapshot = default!;

    protected override async Task OnInitializedAsync()
    {
        userTraysSnapshot = await GetByUserAsync(SessionAccount.User.Id);

        isRendered = true;
    }

    private async Task<UserTraysSnapshotResponse?> GetByUserAsync(string userId)
    {
        try
        {
            var userTraysSnapshotResponse = await HttpRepository.GetAsync<UserTraysSnapshotResponse>($"api/dashboards/user-trays-snapshot/{userId}");

            if (userTraysSnapshotResponse.Error)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el resumen de bandejas");
            }

            return userTraysSnapshotResponse.Response!;
        }
        catch (Exception)
        {
            await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener el resumen de bandejas");
            return null;
        }
    }
}