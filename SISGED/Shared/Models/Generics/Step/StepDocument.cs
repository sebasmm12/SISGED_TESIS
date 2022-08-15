namespace SISGED.Shared.Models.Generics.Step
{
    public class StepDocument
    {
        public StepDocument(int index)
        {
            Index = index;
        }

        public StepDocument()
        {

        }

        public string Uid { get; set; } = default!;
        public int Index { get; set; } = default!;
        public string Type { get; set; } = default!;
        public List<DocumentStep> Steps { get; set; } = new();
    }
}
