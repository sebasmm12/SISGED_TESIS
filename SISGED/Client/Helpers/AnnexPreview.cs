namespace SISGED.Client.Helpers
{
    public class AnnexPreview
    {
        public AnnexPreview(string icon, string color, IEnumerable<string> extensions)
        {
            Icon = icon;
            Color = color;
            Extensions = extensions;
        }

        public AnnexPreview(string icon, string color) : this(icon, color, new List<string>())
        {
            
        }

        public string Icon { get; set; } = default!;
        public string Color { get; set; } = default!;
        public IEnumerable<string> Extensions { get; set; } = default!;

        
        
    }
}
