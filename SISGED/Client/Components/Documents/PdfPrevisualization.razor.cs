using Microsoft.AspNetCore.Components;

namespace SISGED.Client.Components.Documents
{
    public partial class PdfPrevisualization
    {
        [Parameter]
        public string Url { get; set; } = default!;
    }
}