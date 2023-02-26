using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;

namespace SISGED.Client.Components.Documents.Informations
{
    public partial class AttachedDocumentInfo
    {
        [Parameter]
        public string Url { get; set; } = default!;
        [Parameter]
        public string Name { get; set; } = default!;
        [Parameter]
        public AnnexPreview Icon { get; set; } = default!;
    }
}