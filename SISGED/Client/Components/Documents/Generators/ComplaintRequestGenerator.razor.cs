using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Helpers;
using SISGED.Shared.DTOs;
using System.Text.Json;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class ComplaintRequestGenerator
    {        

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;

        private ComplaintRequestContentDTO complaintRequestContent = default!;

        protected override void OnInitialized()
        {
            complaintRequestContent = JsonSerializer.Deserialize<ComplaintRequestContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content))!;
        }
    }
}