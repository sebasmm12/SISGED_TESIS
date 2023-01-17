using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class ToolWindowStrategy
    {
        private readonly IEnumerable<IToolWindowRender> toolWindows = new List<IToolWindowRender>()
        {
            new DocumentRegisterRender(),
            new DocumentSendRender(),
            new DocumentGenerationRender()
        };

        public IToolWindowRender GetToolWindow(string toolName)
        {
            return toolWindows.FirstOrDefault(x => x.ToolName == toolName)!;
        }
    }
}
