using MudBlazor;

namespace SISGED.Client.Helpers
{
    public class BadgePreview
    {
        public BadgePreview() {  }

        public BadgePreview(Color color, string icon, bool selected)
        {
            Color = color;
            Icon = icon;
            Selected = selected;
        }

        public Color Color { get; set; }
        public string Icon { get; set; } = default!;
        public bool Selected { get; set; } = default!;
    }
}
