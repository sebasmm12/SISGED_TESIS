using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;

namespace SISGED.Client.Components.Documents
{
    public partial class AnnexPrevisualization
    {

        [Parameter]
        public AnnexPreview AnnexPreview { get; set; } = default!;

        [Parameter]
        public string AnnexName { get; set; } = default!;

        [Parameter]
        public EventCallback<string> DeleteAnnex { get; set; } = default!;

        private async Task DeleteAnnexAsync()
        {
            await DeleteAnnex.InvokeAsync(AnnexName);
        }
    }
}