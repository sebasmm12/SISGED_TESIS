using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Generics;

public partial class GenericStackedColumnChart
{
    [Inject]
    public IHttpRepository HttpRepository { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    public ISwalFireRepository SwalFireRepository { get; set; } = default!;

    [Parameter]
    public object Data { get; set; } = default!;

    [Parameter]
    public string XAxisLabel { get; set; } = default!;

    [Parameter]
    public string YAxisLabel { get; set; } = default!;

    private bool chartLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var columnChartModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "../js/charts/stacked-column-chart.js");

        await columnChartModule.InvokeVoidAsync("drawStackedColumnChart", Data, XAxisLabel, YAxisLabel);

        chartLoading = false;
    }

}