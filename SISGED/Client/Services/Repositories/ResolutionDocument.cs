using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Registers;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class ResolutionDocument : IDocumentRender
    {
        public string DocumentType { get; set; } = "Resolution";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(ResolutionRegister));
            builder.CloseComponent();
        };
    }
}
