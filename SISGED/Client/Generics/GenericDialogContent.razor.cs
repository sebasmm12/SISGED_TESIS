using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SISGED.Client.Generics
{
    public partial class GenericDialogContent<TValue>
    {

        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public string Body { get; set; } = default!;

        [Parameter] public Task<TValue> DialogMethod { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            TValue result = await DialogMethod;

            MudDialog.Close(DialogResult.Ok(result));
        }
    }
}