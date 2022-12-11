namespace SISGED.Client.Helpers
{
    public class WorkToolPlace
    {
        public WorkToolPlace(string currentPlace, string newPlace, Item? item = null)
        {
            CurrentPlace = currentPlace;
            NewPlace = newPlace;
            Item = item;
        }

        public string CurrentPlace { get; set; } = default!;
        public string NewPlace { get; set; } = default!;
        public Item? Item { get; set; }
    }
}
