
namespace SISGED.Shared.Models.Requests.Step
{
    public class StepsUpdateRequest
    {
        public Int32 Index { get; set; }
        public String Name { get; set; } = default!;
        public String Description { get; set; } = default!;
        public DateTime? StartDate { get; set; } = default!;
        public DateTime? EndDate { get; set; } = default!;
        public DateTime? DueDate { get; set; } = default!;
        public Int32 Days { get; set; }
        public String DossierId { get; set; } = default!;
        public Int32 Step { get; set; }
        public Int32 Substep { get; set; }
        public String DocumentType { get; set; } = default!;
        public Int32 OldStep { get; set; }
        public String OldDocumentType { get; set; } = default!;
    }
}
