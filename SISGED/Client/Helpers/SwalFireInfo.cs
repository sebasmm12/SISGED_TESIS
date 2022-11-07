namespace SISGED.Client.Helpers
{
    public class SwalFireInfo
    {
        public SwalFireInfo(string title, string html, SwalFireIcons icon, string? action = null)
        {
            Title = title;
            Action = action;
            Html = html;
            Icon = Enum.GetName(typeof(SwalFireIcons), icon)!.ToLower();
        }

        public string Title { get; set; } = default!;
        public string? Action { get; set; } = default!;
        public string Html { get; set; } = default!;
        public string Icon { get; set; } = default!;
    }
}
