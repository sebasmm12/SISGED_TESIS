using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Generics
{
    public partial class GenericDialogContent
    {

        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public string Body { get; set; } = default!;

        [Parameter] public Task<bool> DialogMethod { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            bool result = await DialogMethod;

            MudDialog.Close(DialogResult.Ok(result));
        }
    }
}