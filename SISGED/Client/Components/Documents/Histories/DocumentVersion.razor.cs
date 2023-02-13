using Microsoft.AspNetCore.Components;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.DocumentVersion;

namespace SISGED.Client.Components.Documents.Histories
{
    public partial class DocumentVersion
    {
        [Parameter]
        public DocumentVersionInfo ContentVersion { get; set; } = default!;
    }
}