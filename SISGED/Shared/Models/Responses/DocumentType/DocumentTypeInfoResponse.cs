namespace SISGED.Shared.Models.Responses.DocumentType
{
    public class DocumentTypeInfoResponse
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public override bool Equals(object? obj)
        {
            var other = obj as DocumentTypeInfoResponse;
            return other?.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
