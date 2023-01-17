using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents.Generators;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DictumGeneration : IDocumentRender
    {
        public string DocumentType { get; set; } = "Dictamen";

        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(DictumGenerator));
            builder.CloseComponent();
        };
    }
}
