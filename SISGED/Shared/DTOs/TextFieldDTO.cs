namespace SISGED.Shared.DTOs
{
    public class TextFieldDTO
    {
        public TextFieldDTO(string description, int index)
        {
            Description = description;
            Index = index;
        }

        public string Description { get; set; } = default!;
        public int Index { get; set; }
    }
}
