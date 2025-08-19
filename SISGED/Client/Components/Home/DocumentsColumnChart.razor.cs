using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Dashboards;

namespace SISGED.Client.Components.Home;

public partial class DocumentsColumnChart
{

    [Inject]
    public IHttpRepository HttpRepository { get; set; } = default!;

    [Inject]
    public ISwalFireRepository SwalFireRepository { get; set; } = default!;

    private IEnumerable<RoleDocumentsResponse> roleDocuments = default!;

    private bool documentsLoading = true;
    private string xAxisLabel = default!;
    private string yAxisLabel = default!;

    private readonly IEnumerable<DateFilterItem> dateFilterItems = new List<DateFilterItem>
    {
        new (DateFilterDTO.Daily, "Últimos 7 días"),
        new (DateFilterDTO.Monthly, "Últimos 30 días"),
        new (DateFilterDTO.Yearly, "Últimos 12 meses")
    };

    private string selectedDateFilterText = "Últimos 7 días";

    protected override async Task OnInitializedAsync()
    {
        roleDocuments = await GetDocumentsByRoleAsync(DateFilterDTO.Daily);

        xAxisLabel = nameof(RoleDocumentsResponse.Role).ToLower();
        yAxisLabel = nameof(RoleDocumentsResponse.Documents).ToLower();

        documentsLoading = false;
    }


    private async Task SearchDocumentsByRoleAsync(DateFilterItem dateFilterItem)
    {
        documentsLoading = true;

        roleDocuments = await GetDocumentsByRoleAsync(dateFilterItem.DateFilter);

        selectedDateFilterText = dateFilterItem.Name;

        documentsLoading = false;

    }

    private async Task<IEnumerable<RoleDocumentsResponse>> GetDocumentsByRoleAsync(DateFilterDTO dateFilter)
    {
        try
        {
            var roleDocumentsResponse = await HttpRepository.GetAsync<IEnumerable<RoleDocumentsResponse>>($"api/dashboards/roles/documents/{dateFilter}");

            if (roleDocumentsResponse.Error)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos por role para las gráficas");
            }

            var response = roleDocumentsResponse.Response!;

            return response;

        }
        catch (Exception)
        {
            await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos por role para las gráficas");
            return Enumerable.Empty<RoleDocumentsResponse>();
        }
    }
}