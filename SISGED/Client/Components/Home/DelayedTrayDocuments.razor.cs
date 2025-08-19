using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Client.Components.Home;

public partial class DelayedTrayDocuments
{
    [Inject]
    public IHttpRepository HttpRepository { get; set; } = default!;

    [Inject]
    public ISwalFireRepository SwalFireRepository { get; set; } = default!;

    [CascadingParameter(Name = "SessionAccount")]
    public SessionAccountResponse SessionAccount { get; set; } = default!;

    private bool requestsLoading = true;

    private async Task<TableData<ExpiredTrayDocuments>> LoadTableAsync(TableState state)
    {
        var expiredTrayDocumentsResponse = await GetExpiredTrayDocumentsAsync(SessionAccount.GetUser().Id);

        return new()
        {
            Items = expiredTrayDocumentsResponse.ExpiredTrayDocuments,
            TotalItems = expiredTrayDocumentsResponse.ExpiredTrayDocuments.Count()
        };
    }

    private async Task<ExpiredTraysDocumentsResponse> GetExpiredTrayDocumentsAsync(string userId)
    {
        try
        {
            var expiredTrayDocumentsResponse = await HttpRepository.GetAsync<ExpiredTraysDocumentsResponse>($"api/dashboards/expired-trays-documents/{userId}");

            if (expiredTrayDocumentsResponse.Error)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se tiene ningún documento registrado en sus bandejas");
            }

            if (requestsLoading) requestsLoading = false;

            return expiredTrayDocumentsResponse.Response!;
        }
        catch (Exception e)
        {
            await SwalFireRepository.ShowErrorSwalFireAsync("No se tiene ningún documento registrado en sus bandejas");

            return new(new List<ExpiredTrayDocuments>());
        }
    }
}