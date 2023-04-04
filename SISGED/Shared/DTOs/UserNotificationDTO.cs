namespace SISGED.Shared.DTOs
{
    public class UserNotificationDTO
    {
        public UserNotificationDTO(string name, string area, string documentTitle)
        {
            Name = name;
            Area = area;
            DocumentTitle = documentTitle;
        }

        public string Name { get; set; } = default!;
        public string Area { get; set; } = default!;
        public string DocumentTitle { get; set; } = default!;
    }
}
