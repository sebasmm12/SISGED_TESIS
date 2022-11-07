using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Repositories;

namespace SISGED.Client.Components.WorkEnvironments
{
    public partial class ToolWindow
    {
        [Inject]
        public ToolWindowStrategy ToolWindowStrategy { get; set; } = default!;

        [Parameter]
        public Item DraggedItem { get; set; } = default!;
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            var toolWindowRender = ToolWindowStrategy.GetToolWindow(DraggedItem.Name);

            ChildContent = toolWindowRender.RenderFragment;
        }
    }
}