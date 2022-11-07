using Microsoft.AspNetCore.Components;
using SISGED.Client.Components.Documents;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class DocumentSendRender : IToolWindowRender
    {
        public string ToolName { get; set; } = "Derivar Documento";
        public RenderFragment RenderFragment { get; set; } = builder =>
        {
            builder.OpenComponent(0, typeof(DocumentSend));
            builder.CloseComponent();
        };
    }
}
