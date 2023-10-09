using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Generators;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class SessionResolutionGeneration : IDocumentRender
    {
        public string DocumentType { get; set; } = "ResolucionSesion";

        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(SessionResolutionGenerator));
            builder.CloseComponent();
        };
    }
}
