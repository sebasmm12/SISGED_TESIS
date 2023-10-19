namespace SISGED.Shared.DTOs
{
    public class RegisteredDocumentDTO
    {
        public RegisteredDocumentDTO(string id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        public string Id { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime DueDate { get; set; } = default!;

        public void SetDueDate(int totalDays)
        {
            DueDate = CreationDate.AddDays(totalDays).Date;
        }
    }
}
