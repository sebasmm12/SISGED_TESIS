using MudBlazor;

namespace SISGED.Client.Helpers
{
    public class MultipleTextIcon
    {
        public MultipleTextIcon(string icon, Color color)
        {
            Icon = icon;
            Color = color;
        }

        public string Icon { get; set; } = default!;
        public Color Color { get; set; } = default!;
    }
}
