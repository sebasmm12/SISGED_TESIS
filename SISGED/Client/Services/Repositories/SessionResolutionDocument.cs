using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class SessionResolutionDocument :  IDocumentRender
    {
        public string DocumentType { get; set; } = "SessionResolution";

        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(SessionResolutionRegister));
            builder.CloseComponent();
        };
    }
}
