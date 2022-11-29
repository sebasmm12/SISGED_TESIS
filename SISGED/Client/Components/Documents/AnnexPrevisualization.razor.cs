using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SISGED.Client.Helpers;
using SISGED.Shared.DTOs;

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
           // Console.WriteLine(Position);
            await DeleteAnnex.InvokeAsync(AnnexName);
        }
    }
}