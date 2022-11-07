namespace SISGED.Client.Helpers
{
    public class DocumentOption
    {
        public DocumentOption(string label, string value, Roles rol)
        {
            Label = label;
            Value = value;
            Rol = rol;
        }

        public string Label { get; set; } = default!;
        public string Value { get; set; } = default!;
        public Roles Rol { get; set; }
    }
}
