using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Generics.Step
{
    public class DocumentStep
    {
        public string Uid { get; set; } = default!;
        public int Index { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Days { get; set; }
        public List<Substep> Substep { get; set; } = default!;
    }
}
