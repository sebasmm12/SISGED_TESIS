namespace SISGED.Client.Helpers
{
    public class DialogParameter
    {
        public DialogParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; } = default!;
        public object Value { get; set; } = default!;

        
    }
}
