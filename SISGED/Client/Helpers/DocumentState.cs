using MudBlazor;

namespace SISGED.Client.Helpers
{
    public class DocumentState
    {
        public Color Color { get; set; } = default!;
        public string ProcessName { get; set; } = default!;

        public DocumentState(Color color, string processName)
        {
            Color = color;
            ProcessName = processName;
        }
    }
}
