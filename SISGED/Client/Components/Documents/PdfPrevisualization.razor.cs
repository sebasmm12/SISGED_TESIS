using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SISGED.Client.Components.Documents
{
    public partial class PdfPrevisualization
    {
        [CascadingParameter]
        public MudDialogInstance PrevisualizationDialog { get; set; } = default!;

        [Parameter]
        public string Url { get; set; } = default!;

        private void Cancel()
        {
            PrevisualizationDialog.Cancel();
        }

        private void Generate()
        {
            PrevisualizationDialog.Close(DialogResult.Ok(true));
        }
    }
}