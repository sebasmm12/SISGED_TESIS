using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Client.Components.Home;
public partial class UserDocumentMultiColumnChart
{
    [Inject]
    public IHttpRepository HttpRepository { get; set; } = default!;

    [Inject]
    public ISwalFireRepository SwalFireRepository { get; set; } = default!;

    private IEnumerable<UserDocumentHistoryStateResponse> userDocuments = default!;

    [CascadingParameter(Name = "SessionAccount")]
    public SessionAccountResponse SessionAccount { get; set; } = default!;

    private bool documentsLoading = true;
    private string xAxisLabel = default!;
    private string yAxisLabel = default!;

    private readonly IEnumerable<DateFilterItem> dateFilterItems = new List<DateFilterItem>
    {
        new (DateFilterDTO.Daily, "Hoy"),
        new (DateFilterDTO.Monthly, "Mes"),
        new (DateFilterDTO.Yearly, "Año")
    };

    private string selectedDateFilterText = "Hoy";

    protected override async Task OnInitializedAsync()
    {
        userDocuments = await GetUserDocumentsByStateAsync(DateFilterDTO.Daily, SessionAccount.User.Id);

        xAxisLabel = nameof(UserDocumentHistoryStateResponse.Date).ToLower();
        yAxisLabel = nameof(UserDocumentHistoryStateResponse.Documents).ToLower();

        documentsLoading = false;
    }

    private async Task SearchUserDocumentsByStateAsync(DateFilterItem dateFilterItem)
    {
        documentsLoading = true;

        userDocuments = await GetUserDocumentsByStateAsync(dateFilterItem.DateFilter, SessionAccount.User.Id);

        selectedDateFilterText = dateFilterItem.Name;

        documentsLoading = false;

    }

    private async Task<IEnumerable<UserDocumentHistoryStateResponse>> GetUserDocumentsByStateAsync(DateFilterDTO dateFilter, string userId = "")
    {
        try
        {
            string userRequestQueries = GetQueriesForRequest(userId, dateFilter);
            var userDocumentsResponse = await HttpRepository.GetAsync<IEnumerable<UserDocumentHistoryStateResponse>>($"api/dashboards/user-document-history-state{userRequestQueries}");

            if (userDocumentsResponse.Error)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos por estado para las gráficas");
            }

            var response = userDocumentsResponse.Response!;

            return response;

        }
        catch (Exception)
        {
            await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos por estado para las gráficas");
            return Enumerable.Empty<UserDocumentHistoryStateResponse>();
        }
    }

    private static string GetQueriesForRequest(string userId, DateFilterDTO dateFilter)
    {
        string userRequestQueries = "?";

        userRequestQueries += $"UserId={System.Web.HttpUtility.UrlEncode(userId)}";
        userRequestQueries += $"&DateFilter={System.Web.HttpUtility.UrlEncode(((int)dateFilter).ToString())}";

        return userRequestQueries;
    }
}
