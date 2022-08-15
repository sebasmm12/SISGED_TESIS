namespace SISGED.Shared.Entities
{
    public class LegalAct
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<Contract> Contracts { get; set; } = default!;
        public List<Grantor> Grantors { get; set; } = default!;
    }
}
