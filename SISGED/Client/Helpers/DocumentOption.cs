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


        public override bool Equals(object? obj)
        {
            var other = obj as DocumentOption;
            return other?.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
