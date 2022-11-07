using Microsoft.AspNetCore.Components;

namespace SISGED.Client.Services.Contracts
{
    public interface IToolWindowRender
    {
        public string ToolName { get; set; }
        public RenderFragment RenderFragment { get; set; }
    }
}
