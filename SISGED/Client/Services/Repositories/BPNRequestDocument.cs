using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class BPNRequestDocument : IDocumentRender
    {
        public string DocumentType { get; set; } = "BPNRequest";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(BPNRequestRegister));
            builder.CloseComponent();
        };
    }
}
