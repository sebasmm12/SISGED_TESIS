namespace SISGED.Client.Helpers
{
    public class SelectOption
    {
        public SelectOption(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; set; } = default!;
        public string Value { get; set; } = default!;

        public override bool Equals(object? obj)
        {
            var other = obj as SelectOption;
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
