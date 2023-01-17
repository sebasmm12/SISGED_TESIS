using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentGenerationRender : IToolWindowRender
    {
        public string ToolName { get; set; } = "Generar Documento";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(DocumentGeneration));
            builder.CloseComponent();
        };
    }
}
