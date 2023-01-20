using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Generators;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class ResolutionGeneration : IDocumentRender
    {
        public string DocumentType { get; set; } = "Resolucion";

        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(ResolutionGenerator));
            builder.CloseComponent();
        };
    }
}
