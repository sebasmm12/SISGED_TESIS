
namespace SISGED.Shared.Models.Requests.Step
{
    public class StepsUpdateRequest
    {
        public int Index { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime? StartDate { get; set; } = default!;
        public DateTime? EndDate { get; set; } = default!;
        public DateTime? DueDate { get; set; } = default!;
        public int Days { get; set; }
        public string DossierId { get; set; } = default!;
        public int Step { get; set; }
        public int Substep { get; set; }
        public string DocumentType { get; set; } = default!;
        public int OldStep { get; set; }
        public string OldDocumentType { get; set; } = default!;
    }
}
