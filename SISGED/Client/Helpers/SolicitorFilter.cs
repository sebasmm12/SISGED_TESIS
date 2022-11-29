namespace SISGED.Client.Helpers
{
    public class SolicitorFilter
    {
        public SolicitorFilter(string? solicitorName, bool? exSolicitor)
        {
            SolicitorName = solicitorName;
            ExSolicitor = exSolicitor;
        }

        public string? SolicitorName { get; set; }
        public bool? ExSolicitor { get; set; }
    }
}
