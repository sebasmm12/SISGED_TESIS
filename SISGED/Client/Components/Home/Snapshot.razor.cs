using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SISGED.Client.Components.Home;

public partial class Snapshot
{
    [Parameter]
    public string Title { get; set; } = default!;

    [Parameter]
    public int Quantity { get; set; } = default!;

    [Parameter] 
    public string Icon { get; set; } = default!;

    [Parameter]
    public string Color { get; set; } = default!;

    [Parameter]
    public string BackgroundColor { get; set; } = default!;
}